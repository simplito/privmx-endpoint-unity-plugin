# PrivMX Endpoint Unity Plugin

This repository provides Unity Plugin used by PrivMX to handle end-to-end (e2e) encryption.

PrivMX is a privacy-focused platform designed to offer secure collaboration solutions by integrating robust encryption across various data types and communication methods.
This project enables seamless integration of PrivMX’s encryption functionalities in Unity Engine applications, preserving the security and performance of the original C++ library while making its capabilities accessible in the Unity projects.

## About PrivMX

[PrivMX](https://privmx.dev) allows developers to build end-to-end encrypted apps used for communication. The Platform works according to privacy-by-design mindset, so all of our solutions are based on Zero-Knowledge architecture. This project extends PrivMX’s commitment to security by making its encryption features accessible to developers using C# in Unity Engine.

#### Key Features
- End-to-End Encryption: Ensures that data is encrypted at the source and can only be decrypted by the intended recipient.
- Native C++ Library Integration: Leverages the performance and security of C++ while making it accessible in Unity applications.
- Cross-Platform Compatibility: Designed to support PrivMX on multiple operating systems and environments.
- Simple API: Easy-to-use interface for Unity developers without compromising security.

##  Package

PrivMX Endpoint Unity Plugin provides functionality of [PrivMX Endpoint C#](https://github.com/simplito/privmx-endpoint-csharp) and [PrivMX Endpoint C# Extra](https://github.com/simplito/privmx-endpoint-csharp-extra), wrapping it all into a simple, ready to use package in Unity Engine. It adds utility data structures that simplify working with state and event processing and helps developers with base configuration of PrivMX Session. 

Features:

- `CryptoApi` - Cryptographic methods used to encrypt/decrypt and sign your data or generate keys to work with PrivMX Bridge.
- `Connection` - Methods for managing connection with PrivMX Bridge.
- `ThreadApi` - Methods for managing Threads and sending/reading messages.
- `StreamApi` - Methods for managing sending/receiving data via Streams.

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

In your Unity project, go to **Window -> Package Manager**, select the **"Install package from git URL"** option in **+** menu tab and paste: `https://github.com/simplito/privmx-endpoint-unity-plugin.git`.

## Getting Started

#### Project Configuration

1. Make sure that project is configured to use **Mono** as scripting backend.
2. Configure PrivMX Bridge.

#### Setup Example
Example bellow shows how to join and manage a PrivMX Session. It assumes that there already is a **context** with at least one **user** added to it.
To create your users and contexts use the [Bridge API](https://bridge.privmx.dev/#privmx-bridge-api) in your code.

1. Create a empty GameObject and add **PrivMX Session** and **SessionStateChange_EventDispatcher** to it.
<img width="675" height="595" alt="image" src="https://github.com/user-attachments/assets/61108932-9fe3-4cb1-8e05-880028421aa3" />

2. Create a script that will handle PrivMX related logic.
3. Create a GenerateKeyPair method and add a method that will allow the app to generate a private and public key for the user, that wants to join the session.

```csharp
using Simplito;
using UnityEngine;

public class PrivMX_Example : MonoBehaviour
{
[SerializeField] private PrivMxSession session;
private string publicKey;
private string privateKey;

  public void GenerateKeyPair(string password, string salt)
  {
      privateKey = PrivMXUtility.DerivePrivateKey(password, salt);
      publicKey = PrivMXUtility.DerivePublicKey(privateKey);
  }
}
```
  
4. To connect to your Bridge, create a **ConnectToPrivmx()** method, that will authorize the user. If function returns **true**, user is logged in.

```csharp
public async Task<bool> ConnectToPrivmx(string bridgeAddress, string solutionId, CancellationToken token = default)
{
    try
    {
        using (token.LinkIfNeeded(destroyCancellationToken, out token))
        {
            token.ThrowIfCancellationRequested();
            await session.Authorize(bridgeAddress, solutionId, privateKey, token);
        }
        return true;
    }
    catch (Exception ex)
    {
        Debug.Log("Authorize failed: " + ex.Message);
    }
    return false;
}

public async void JoinSession(string bridgeAddress, string solutionId)
{
    bool loggedIn = await ConnectToPrivmx(bridgeAddress, solutionId);

    if (loggedIn) Debug.Log("Logged in");
    else Debug.Log("Unable to log in");
}
```

5. To disconnect the user, create a **LogOutOfSession()** method.
```csharp
public void LogOutOfSession()
{
    try
    {
        session.DisconnectAsync();
    }
    catch (Exception exception)
    {
        Debug.LogException(exception);
    }
}
```

6. Now, in the Inspector, add **callbacks** to your SessionStateChange_EventDispatcher. First one should be called when a user joins session successfully. The other when user logs out.
```csharp
public void JoinedSessionCallback()
{
    Debug.Log("Logged in");
}
public void LoggedOutCallback()
{
    Debug.Log("Logged out");
}
```
It should look like this:

<img width="677" height="716" alt="image" src="https://github.com/user-attachments/assets/dbf90679-6c47-4db2-a6b3-eeae120da266" />

## License information

**PrivMX Endpoint C#**\
Copyright © 2024 Simplito sp. z o.o.

This project is part of the PrivMX Platform (https://privmx.dev). \
This project is Licensed under the MIT License.

PrivMX Endpoint and PrivMX Bridge are licensed under the PrivMX Free License.\
See the License for the specific language governing permissions and limitations under the License.
