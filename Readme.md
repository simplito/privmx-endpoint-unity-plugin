# Simplito PrivMX

> TODO: Introduction to product

# Requirements

At least Unity `2022.3.x`.

# Installation

Library can be installed in various ways:

## As git repository with package manager (recommended)

> TODO: Write this section after initial migration to Github

## By coping source to `Packages` directory 
Library can be installed in project by copying this repository contents into `Packages` directory in Unity project.

# Core interface
Main entry point for the library is the [PrivMXSession](./SimplitoPrivMX/PrivMXSession.cs) MonoBehaviour class.
`PrivMXSession` encapsulates session of single authenticated user.
Most of public methods in this class require user authentication before use.
        
Most parts of the public API are asynchronous operations either methods returning [Task\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-8.0)/[ValueTask\<T\>](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask?view=net-8.0) or [IObservable\<T\>]()

It's recommended to use [R3](https://github.com/Cysharp/R3) to simplify use of `IObservable<T>` returned from library.


# Getting started

> TODO: Sample of how to move from login flow to sending message in thread
 
# Samples

If the `R3` library is not an option then there are utility classes included in the library samples that should simplify interaction with observables in Unity.

![](./Documentation~/utility_sample.png)


