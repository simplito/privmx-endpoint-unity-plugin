# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

### Added

- new static methods `ConnectAsync` and `ConnectPublicAsync` in `ConnectionAsync` static class

- new optional parameter of type `ContainerPolicy` in `CreateStoreAsync` and `UpdateStoreAsync` methods in `StoreApiAsync` static class

- new optional parameter of type `ContainerPolicy` in `CreateThreadAsync` and `UpdateThreadAsync` methods in `ThreadApiAsync` static class

### Changed

- updated PrivMXEndpointCsharp to unknown version

- updated native endpoint libraries for Windows

### Deprecated

- deprecated `PlatformConnectAsync` and `PlatformConnectPublicAsync` in `ConnectionAsync`