Unity PrimMX plugin is a Unity package that simplifies integration with PrinMX platform.
This plugin is built on top of Simplito PrivMX C# library [privmx-endpoint-csharp](https://github.com/simplito/privmx-endpoint-csharp).

Aim of the Unity plugin is to:
- integrate the C# library and native libraries into Unity workflow,
- add utility data structures that simplify working with state and event processing,
- add support for an asynchronous concurrent method call to C# library.

It's optional to use data structures from the Unity plugin, user can always decide to only use the [privmx-endpoint-csharp](https://github.com/simplito/privmx-endpoint-csharp) and use the Unity plugin as a convenient way of integrating C# endpoint with unity game engine.


The plugin is distributed as:
- [public Github repository](https://github.com/simplito/privmx-endpoint-unity-plugin)

## Requirements
- Unity (officially supported are all Unity LTS distributions since 2022.3),
- working instance of PrivMX bridge.

## Supported Unity platforms
Plugin supports:
- development in Unity Editor on Windows,
- standalone builds on Windows that use Mono as scripting backend,
- mobile builds on Android devices that use Mono as scripting backed.

## Demo projects
- [privmx-chatee-unity](https://github.com/simplito/privmx-chatee-unity) - project that demonstrates how to create a chat in Unity application. This project cooperates with [privmx-chatee](https://github.com/simplito/privmx-chatee),
- [privmx-chatee-unity-vr](https://github.com/simplito/privmx-chatee-unity-vr) - shows how to use Unity plugin to write a chat in VR that integrates with [privmx-chatee](https://github.com/simplito/privmx-chatee).
