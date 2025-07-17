# PrivMX Endpoint Unity Plugin

This repository provides Unity Plugin used by PrivMX to handle end-to-end (e2e) encryption.

PrivMX is a privacy-focused platform designed to offer secure collaboration solutions by integrating robust encryption across various data types and communication methods.
This project enables seamless integration of PrivMX’s encryption functionalities in Unity application, preserving the security and performance of the original C++ library while making its capabilities accessible in the Unity projects.

## About PrivMX

[PrivMX](https://privmx.dev) allows developers to build end-to-end encrypted apps used for communication. The Platform works according to privacy-by-design mindset, so all of our solutions are based on Zero-Knowledge architecture. This project extends PrivMX’s commitment to security by making its encryption features accessible to developers using C# in Unity Engine.

#### Key Features
- End-to-End Encryption: Ensures that data is encrypted at the source and can only be decrypted by the intended recipient.
- Native C++ Library Integration: Leverages the performance and security of C++ while making it accessible in Unity applications.
- Cross-Platform Compatibility: Designed to support PrivMX on multiple operating systems and environments.
- Simple API: Easy-to-use interface for Unity developers without compromising security.

##  Package

PrivMX Endpoint Unity Plugin provides functionality of [PrivMX Endpoint C#](https://github.com/simplito/privmx-endpoint-csharp) and [PrivMX Endpoint C# Extra](https://github.com/simplito/privmx-endpoint-csharp-extra), wrapping it all into a simple, ready to use package in Unity Engine. It helps developers with base configuration of PrivMX Session and allows them to quickly connect and use it's features:

- `CryptoApi` - Cryptographic methods used to encrypt/decrypt and sign your data or generate keys to work with PrivMX Bridge.
- `Connection` - Methods for managing connection with PrivMX Bridge.
- `ThreadApi` - Methods for managing Threads and sending/reading messages.
- `StoreApi` - Methods for managing Stores and sending/reading files.
- `InboxApi` - Methods for managing Inboxes and entries.

## Requirements

- At least Unity `2022.3.x`.
- Access to running [PrivMX Bridge](https://github.com/simplito/privmx-bridge).

## Supported Platforms
- Android
  - `arm64-v8a`
  - `armeabi-v7a`
- Windows
  - `x86_64` 

## Installation

In Unity, go to Window->Package Managera and select the "Install package from git URL" option in **+** menu tab.

## Documentation

Documentation of how to install and use this plugin is available in the [Documentation~](./Documentation~) directory.

## License information

**PrivMX Endpoint C#**\
Copyright © 2024 Simplito sp. z o.o.

This project is part of the PrivMX Platform (https://privmx.dev). \
This project is Licensed under the MIT License.

PrivMX Endpoint and PrivMX Bridge are licensed under the PrivMX Free License.\
See the License for the specific language governing permissions and limitations under the License.
