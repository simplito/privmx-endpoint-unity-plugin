# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.3.8] - 2025-07-21

## Added

- CancellationTokenExtensions.cs
- NullDisposable.cs

## [2.3.7] - 2025-07-21

## Added

- SessionStateChangeEventDispatcher.cs
- SynchronizationContextHelper.cs

## [2.3.6] - 2025-07-11

## Changed

- updated android libs to version 2.3.4

## [2.3.5] - 2025-06-30

## Added

- Internal observable extensions class.

### Changed

- PrivMX session script now tracks lib connection.

## [2.3.4] - 2025-06-25

### Changed

- updated C# wrapper to version 2.3.4

## [2.3.3] - 2025-06-25

### Changed

- updated C# wrapper to version 2.3.3

## [2.3.2] - 2025-06-25

### Changed

- updated C# wrapper to version 2.3.2

## [0.0.6] - 2025-06-18

### Changed

- updated native libraries to version 2.3.4 for Windows and Android

- updated C# wrapper to version 2.3

## [0.0.4] - 2025-02-03

### Changed

- updated native libraries to version 2.2 for Windows and Android

- updated C# wrapper to version 2.1.1

- `IConnection.GetInstanceId` changed name to `IConnection.GetConnectionId`

## [0.0.3] - 2024-12-09

### Changed

- updated PrivMXEndpointCsharp native libraries for android v7 and 64

### Fixed

- fixed event dispatch for threads and thread messages
   
## [0.0.2] - 2024-12-05

### Added

- new static methods `ConnectAsync` and `ConnectPublicAsync` in `ConnectionAsync` static class

- new optional parameter of type `ContainerPolicy` in `CreateStoreAsync` and `UpdateStoreAsync` methods in `StoreApiAsync` static class

- new optional parameter of type `ContainerPolicy` in `CreateThreadAsync` and `UpdateThreadAsync` methods in `ThreadApiAsync` static class

### Changed

- updated PrivMXEndpointCsharp to version 2.1.1

- updated native endpoint libraries for Windows

### Deprecated

- deprecated `PlatformConnectAsync` and `PlatformConnectPublicAsync` in `ConnectionAsync`
