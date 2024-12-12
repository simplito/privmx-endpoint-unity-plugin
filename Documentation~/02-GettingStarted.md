## Before you start

Before you start developing end-to-end application, you need to have working instance of [PrivMX bridge](/#bridge). 
To connect it to your environment you will need these API keys:
    - `SolutionID` - created inside your Organization,
    - `Platform URL` - unique for your Instance,
    - `ContextID` - there can be one or more depending on the use case. 

For more information about how PrivMX Endpoint integrates into your stack check our [getting started guide](/).

## Installation
Plugin can be installed in a Unity project in various ways.
There are three independent ways of installing Unity plugin below. You can choose any of them.

### Installing from github
1. Open `Package Manager` window in Unity Editor (from top bar select `Window` -> `Package Manager`).
2. Click plus icon in top left corner of the window and select `Install package from git URL...`.
3. Paste `https://github.com/simplito/privmx-endpoint-unity-plugin.git`, after a while plugin should be installed in the project.

Plugin updates can be done later form the `Package Manager` window.

### Adding package to `Packages` directory
1. Clone/clone content of [privmx-endpoint-unity-plugin](https://github.com/simplito/privmx-endpoint-unity-plugin) into `Assets/Packages` directory in your Unity project.

### Adding package manually
1. Copy/clone content of [privmx-endpoint-unity-plugin](https://github.com/simplito/privmx-endpoint-unity-plugin).
2. Open `Package Manager` window in Unity Editor (from top bar select `Window` -> `Package Manager`).
3. Click plus icon in top left corner of the window and select `Install package from disk...`.
4. Select directory with copied/cloned content from step 1.

## Project configuration
Make sure that project is configured to use Mono as scripting backend.

## Most important plugin parts
- After plugin installation and initial project setup, your focus should be put on [PriMXSession](https://github.com/simplito/privmx-endpoint-unity-plugin/blob/main/Scripts/PrivMXSession.cs). This class is responsible for managing a state of a single user in PrivMX bridge and provides access to all features related to threads, messages, and storage form [privmx-endpoint-csharp](https://github.com/simplito/privmx-endpoint-csharp) in both synchronous and asynchronous matter.
- API events are exposed in a structured way as `IObservable` implementation either through [PrivMXEventDispatcher](https://github.com/simplito/privmx-endpoint-unity-plugin/blob/main/Scripts/Api/PrivMXEventDispatcher.cs) or `PrivMXSession`.
- Asynchronous extension methods for `IConnection`, `IEventQueue`, `IStoreApi`, and `IThreadApi` are available under `Simplito` namespace and their sources are available in [DefaultExtensions](https://github.com/simplito/privmx-endpoint-unity-plugin/tree/main/Scripts/DefaultExtensions) on github.