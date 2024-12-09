# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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