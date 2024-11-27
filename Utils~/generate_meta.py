__doc__ = \
"""
This script generates meta files for native libraries for Android (ARM64 and ARMv7) and Windows.
The script has no external dependencies and was tested on python 3.12.

Usage:
1. Delete all files from directory with native libraries: 
  - Plugins/Android/ARM64 for Android:ARM64
  - Plugins/Android/ARMv7 for Android:ARMv7
  - Plugins/Windows/x64 for Windows
2. Copy new version of libraries to target directory
3. Run this script with desired preset
4. Commit changes
"""
import pathlib
from typing import Protocol, Any, Literal

Serializable = None | str | int | float | bool | dict[str, 'Serializable'] | list['Serializable']


class SupportsWrite(Protocol):
    def write(self, data: str) -> Any:
        ...


def yaml_dump(data: dict[str, Serializable], fp: SupportsWrite, *, indentation: str = '  ') -> None:
    """
    Simple yaml formater.

    Args:
        data: data to format as yaml file

    Returns:
        yaml formated string
    """
    indentation_level = -1

    def _write_value(value: Serializable, list_element = False):
        nonlocal indentation_level
        match value:
            case None:
                fp.write('\n')
            case True | False:
                fp.write(" " + str(value).lower() + '\n')
            case str() | float() | int():
                fp.write(f' {value}\n')
            case dict():
                if len(value) == 0:
                    fp.write(' {}\n')
                else:
                    if not list_element:
                        fp.write('\n')
                    _dump_dict(value, list_element)
            case list():
                if len(value) == 0:
                    fp.write(' []\n')
                else:
                    if not list_element:
                        fp.write('\n')
                    _dump_list(value, list_element)
            case _:
                raise TypeError(f"Unhandled value {value} of type {type(value)}")

    def _dump_dict(dictionary: dict[str, Serializable], list_element = False):
        nonlocal indentation_level
        indentation_level += 1
        items = dictionary.items()
        if len(items) == 0:
            raise ValueError("Empty dictionary")
        first = True
        for key, value in items:
            if not isinstance(key, str):
                raise TypeError(f"Keys in dictionary can only be strings, got {key} instead of type {type(key)}")
            if first and list_element:
                fp.write(f' {key}:')
                first = False
            else:
                fp.write(f'{indentation * indentation_level}{key}:')
            _write_value(value)
        indentation_level -= 1

    def _dump_list(collection: list[Serializable], list_element = False):
        nonlocal indentation_level
        if len(collection) == 0:
            raise ValueError("Empty list")
        first = True
        # fp.write(hyphen_indent)
        for element in collection:
            if first and list_element:
                fp.write('-')
                first = False
            else:
                hyphen_indent = (indentation_level + 1) * indentation
                hyphen_indent = hyphen_indent[:-2] + '-'
                fp.write(hyphen_indent)
            _write_value(element, True)
    _dump_dict(data)

AndroidCPU = Literal["ARM64", "ARMv7", "X86", "X86_64"]
AndroidSharedLibraryType = Literal["Executable", "Symbol"]
OSType = Literal["AnyOS", "OSX", "Windows", "Linux"]
EditorCPU = Literal["AnyCPU", "x86_64", "ARM64"]
OSXCPU = EditorCPU | Literal["None"]

def _excluded_platform_data(exAndroid: bool, exEditor: bool, exLinux64: bool, exOSX: bool, exWin: bool, exWin64: bool):
    return {
        "first":
            {"": "Any"},
        "second":
            {
                "enabled": 0,
                "settings": {
                    "Exclude Android": int(exAndroid),
                    "Exclude Editor": int(exEditor),
                    "Exclude Linux64": int(exLinux64),
                    "Exclude OSXUniversal": int(exOSX),
                    "Exclude Win": int(exWin),
                    "Exclude Win64": int(exWin64)
                }
            }
    }

def _android_platform_data(enabled: bool, cpu: AndroidCPU, libType: AndroidSharedLibraryType):
    return {
        "first": {
            "Android": "Android"
        },
        "second": {
            "enabled": int(enabled),
            "settings": {
                "AndroidSharedLibraryType": libType,
                "CPU": cpu
            }
        }
    }

def _any_platform_data(enabled: bool):
    return {
        "first": {
            "Any": None
        },
        "second": {
            "enabled": int(enabled),
            "settings": {}
        }
    }

def _editor_platform_data(enabled: bool, editorOS: OSType, editorCPU: EditorCPU):
    return {
        "first": {
            "Editor": "Editor"
        },
        "second": {
            "enabled": int(enabled),
            "settings": {
                "CPU": editorCPU,
                "DefaultValueInitialized": True,
                "OS": editorOS
            }
        }
    }

def _linux_platform_data(enabled: bool):
    return {
        "first": {
            "Standalone": "Linux64"
        },
        "second": {
            "enabled": int(enabled),
            "settings": {
                "CPU": "None"  # Suspicious, that's never changes in editor
            }
        }
    }

def _osx_platform_data(enabled: bool, osxCPU: OSXCPU):
    return {
        "first": {
            "Standalone": "OSXUniversal"
        },
        "second": {
            "enabled": int(enabled),
            "settings": {
                "CPU": "None"  # Suspicious, that's never changes in editor
            }
        }
    }

def _win64_platform_settings(enabled: bool, cpuArch: Literal["None", "AnyCPU"]):
    return {
        "first": {
            "Standalone": "Win64"
        },
        "second": {
            "enabled": int(enabled),
            "settings": {
                "CPU": cpuArch
            }
        }
    }

def _win32_platform_settings(enabled: bool, cpuArch: Literal["None", "AnyCPU"]):
    return {
        "first": {
            "Standalone": "Win"
        },
        "second": {
            "enabled": int(enabled),
            "settings": {
                "CPU": cpuArch
            }
        }
    }

def _gen_uuid() -> str:
    import uuid
    return str(uuid.uuid4()).replace('-', '')


def generate_meta_a(anyPlatform: bool, editorCPU: EditorCPU, editorOS: OSType, androidCPU: AndroidCPU,
                    *excludedPlatforms: Literal["Editor", "Standalone", "Android"]):
    enabledAndroid = "Android" not in excludedPlatforms
    enabledEditor = "Editor" not in excludedPlatforms
    enabledLinux = "Standalone" not in excludedPlatforms
    enabledOSX = "Standalone" not in excludedPlatforms
    enabledWin = "Standalone" not in excludedPlatforms
    enabledWin64 = "Standalone" not in excludedPlatforms
    return {
        "fileFormatVersion": 2,
        "guid": _gen_uuid(),
        "PluginImporter": {
            "externalObjects": {},
            "serializedVersion": 2,
            "iconMap": {},
            "executionOrder": {},
            "defineConstraints": [],
            "isPreloaded": 0,
            "isOverridable": 1,
            "isExplicitlyReferenced": 0,
            "validateReferences": 1,
            "platformData": [
                _excluded_platform_data(exAndroid=not enabledAndroid,
                                        exEditor=not enabledEditor,
                                        exLinux64=not enabledLinux,
                                        exOSX=not enabledOSX,
                                        exWin=not enabledWin,
                                        exWin64=not enabledWin64,
                                        ),
                _android_platform_data(enabledAndroid,
                                       androidCPU,
                                       "Executable"),
                _any_platform_data(anyPlatform),
                _editor_platform_data(enabledEditor, editorOS, editorCPU),
                _linux_platform_data(enabledLinux),
                _osx_platform_data(enabledOSX, "None"),
                _win32_platform_settings(enabledWin, "None"),
                _win64_platform_settings(enabledWin64, "None")
            ],
            "userData": None,
            "assetBundleName": None,
            "assetBundleVariant": None
        }
    }

def generate_meta_so(anyPlatform: bool, os: OSType, cpuArchStandalone: Literal["None", "AnyCPU", "x86_64", "x86"],
                     editorCPU: EditorCPU, androidCPU: AndroidCPU,
                     *excludedPlatforms: Literal["Editor", "Standalone", "Android"]) -> dict[str, Serializable]:
    enabledAndroid = "Android" not in excludedPlatforms
    enabledEditor = "Editor" not in excludedPlatforms
    enabledLinux = os == "AnyOs" or os == "Linux64" and "Standalone" not in excludedPlatforms
    enabledOSX = os == "AnyOs" or os == "OSX" and "Standalone" not in excludedPlatforms
    enabledWin = "Standalone" not in excludedPlatforms and "x86" == cpuArchStandalone
    enabledWin64 = not ("Editor" in excludedPlatforms and "Standalone" in excludedPlatforms) or ("Editor" in excludedPlatforms and "x86" == cpuArchStandalone)
    return {
        "fileFormatVersion": 2,
        "guid": _gen_uuid(),
        "PluginImporter": {
            "externalObjects": {},
            "serializedVersion": 2,
            "iconMap": {},
            "executionOrder": {},
            "defineConstraints": [],
            "isPreloaded": 0,
            "isOverridable": 1,
            "isExplicitlyReferenced": 0,
            "validateReferences": 1,
            "platformData": [
                _excluded_platform_data(exAndroid=not enabledAndroid,
                                        exEditor=not enabledEditor,
                                        exLinux64=not enabledLinux,
                                        exOSX=not enabledOSX,
                                        exWin=not enabledWin,
                                        exWin64=not enabledWin64,
                                        ),
                _android_platform_data(enabledAndroid,
                                       androidCPU,
                                       "Executable"),
                _any_platform_data(anyPlatform),
                _editor_platform_data(enabledEditor, os, editorCPU),
                _linux_platform_data(enabledLinux),
                _osx_platform_data(enabledOSX, "None" if not enabledOSX else "AnyCPU"),
                _win32_platform_settings(enabledWin, "None"),
                _win64_platform_settings(enabledWin64, "None")
            ],
            "userData": None,
            "assetBundleName": None,
            "assetBundleVariant": None
        }
    }

def generate_native_dll_meta(anyPlatform: bool, os: OSType, cpuArchStandalone: Literal["AnyCPU", "x86_64", "x86"],
                             editorCPU: EditorCPU,
                             *excludedPlatforms: Literal["Editor", "Standalone", "Android"]) -> dict[str, Serializable]:
    enabledAndroid = "Android" not in excludedPlatforms
    enabledEditor = "Editor" not in excludedPlatforms
    enabledLinux = os != "AnyOs" and os != "Linux64"
    enabledOSX = os != "AnyOs" and os != "OSX"
    enabledWin = "Standalone" not in excludedPlatforms and "x86" == cpuArchStandalone
    enabledWin64 = (not ("Editor" in excludedPlatforms and "Standalone" in excludedPlatforms)) or ("Standalone" not in excludedPlatforms and  "x86" != cpuArchStandalone)
    return {
        "fileFormatVersion": 2,
        "guid": _gen_uuid(),
        "PluginImporter": {
            "externalObjects": {},
            "serializedVersion": 2,
            "iconMap": {},
            "executionOrder": {},
            "defineConstraints": [],
            "isPreloaded": 0,
            "isOverridable": 1,
            "isExplicitlyReferenced": 0,
            "validateReferences": 1,
            "platformData": [
                _excluded_platform_data(exAndroid=not enabledAndroid,
                                        exEditor=not enabledEditor,
                                        exLinux64=not enabledLinux,
                                        exOSX=not enabledOSX,
                                        exWin=not enabledWin,
                                        exWin64=not enabledWin64,
                                        ),
                _any_platform_data(anyPlatform),
                _editor_platform_data(enabledEditor, os, editorCPU),
                _linux_platform_data(enabledLinux),
                _osx_platform_data(enabledOSX, "None" if not enabledOSX else "AnyCPU"),
                _win32_platform_settings(enabledWin, "AnyCPU"),
                _win64_platform_settings(enabledWin64, "AnyCPU")
            ],
            "userData": None,
            "assetBundleName": None,
            "assetBundleVariant": None
        }
    }

def is_orphaned_meta(path: pathlib.Path) -> bool:
    """
    Returns true if path to meta file doesn't have corresponding source file
    """
    assert path.suffixes[-1] == ".meta"
    master = path.with_suffix("".join(path.suffixes[-1]))
    return not master.exists()

def _handle_android_file(file: pathlib.Path, arch: Literal["ARM64", "ARMv7"]):
    suffixes = set(file.suffixes)
    if ".la" in suffixes:
        file.unlink(True)
        return
    if ({".so", ".meta"} == suffixes or {".a", ".meta"} == suffixes) and is_orphaned_meta(file):
        file.unlink(True)
        return
    if {".so"} == suffixes:
        with file.with_suffix(".so.meta").open('w') as fHandle:
            yaml_dump(
                generate_meta_so(True, "AnyOS", "None", "AnyCPU",
                                 arch,
                                 "Editor", "Standalone"),
                fHandle
            )
        return
    if {".a"} == suffixes:
        with file.with_suffix(".a.meta").open('w') as fHandle:
            yaml_dump(
                generate_meta_a(True, "AnyCPU", "AnyOS", arch,
                                "Editor", "Standalone"),
                fHandle
            )
        return
    if args.debug:
        print(f"Ignored file: {file}")

if __name__ == '__main__':
    import argparse
    ARM64_PATH = pathlib.Path(__file__).parent.parent / "Plugins" / "Android" / "ARM64"
    ARMv7_PATH = pathlib.Path(__file__).parent.parent / "Plugins" / "Android" / "armeabi-v7a"
    WINDOWS64 = pathlib.Path(__file__).parent.parent / "Plugins" / "Windows" / "x64"
    parser = argparse.ArgumentParser(prog="generate_meta.py", description=__doc__, formatter_class=argparse.RawTextHelpFormatter)
    parser.add_argument("preset", help='Platform for which library meta files should be generated/updated.', choices=["Android:ARM64", 'Android:ARMv7', "Windows"])
    parser.add_argument("-d", "--debug", action='store_true', help="Debug mode", required=False)
    args = parser.parse_args()
    if args.preset == "Android:ARM64":
        for root, _, files in ARM64_PATH.walk():
            for file in files:
                file = pathlib.Path(root) / file
                _handle_android_file(file, "ARM64")
    elif args.preset == "Android:ARMv7":
        for root, _, files in ARMv7_PATH.walk():
            for file in files:
                file = pathlib.Path(root) / file
                _handle_android_file(file, "ARMv7")
    elif args.preset == "Windows":
        for root, _, files in WINDOWS64.walk():
            for file in files:
                file = pathlib.Path(root) / file
                suffixes = set(file.suffixes)
                if ".meta" in suffixes and is_orphaned_meta(file):
                    file.unlink(True)
                    continue
                if {".dll"} == suffixes:
                    with file.with_suffix(".dll.meta").open('w') as fHandle:
                        yaml_dump(
                            generate_native_dll_meta(False, "Windows", "x86_64", "AnyCPU", "Android"),
                            fHandle
                        )
                    continue
                if {".dll.a"} == suffixes:
                    with file.with_suffix(".dll.a.meta").open('w') as fHandle:
                        yaml_dump(
                            generate_meta_a(False, "x86_64", "Windows", "ARMv7", "Android"),
                            fHandle
                        )
                if args.debug:
                    print(f"Ignored file: {file}")
    else:
        print("Unknown preset")
        exit(1)
