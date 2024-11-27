__doc__ = \
"""
This script generates documentation so it can be consumed by docsaursus.
Most of the script is a glue code that depends on external cli applications.

External dependencies/assumptions:
- doxygen cli is installed in the os
- xsltproc is installed in the os
- dotnet sdk is installed in the os

Usage:
1. Change C# api xml docstrings.
2. Run this script.
3. Output is saved in in `output/api.json` (relative to this file location)
"""

import subprocess
import argparse
import shutil
from pathlib import Path

SCRIPT_DIRECTORY = Path(__file__).parent
parser = argparse.ArgumentParser(prog="generate_documentation.py", description=__doc__, formatter_class=argparse.RawTextHelpFormatter)
args = parser.parse_args()

# check all executables
doxygen_executable = shutil.which('doxygen')
if doxygen_executable is None:
    raise Exception("doxygen executable not found")
dotnet_sdk = shutil.which('dotnet')
if dotnet_sdk is None:
    raise Exception("dotnet executable not found")
xstlproc_executable = shutil.which('xsltproc')
if xstlproc_executable is None:
    raise Exception("xsltproc executable not found")
# run doxygen
cwd = SCRIPT_DIRECTORY / 'doxygen'
subprocess.run([doxygen_executable, 'Doxyfile'], cwd=cwd, check=True)
# run xstlproc
output = cwd / 'output' / 'xml'
subprocess.run(f'{xstlproc_executable} combine.xslt index.xml > all.xml', shell=True, cwd=output, check=True)
# run postprocessing
cwd = SCRIPT_DIRECTORY / 'doc-generator-cs' / 'DocsGenerator'
assert cwd.exists()
subprocess.run([dotnet_sdk, 'run', str(output / 'all.xml'), str(SCRIPT_DIRECTORY.parent),
                str(output.parent / 'api.json') ], cwd=cwd, check=True)


