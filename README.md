[![Project logo][logo-github]][docs]

[![Codefactor stats][codefactor-badge]][codefactor-stats]
[![AppVeyor build status][appveyor-badge]][appveyor-build-status]
[![AppVeyor build status][nuget-badge]][nuget-package]

> JavaScript-style timers for .NET

Library offers simple API which mimics behaviour of NodeJS/Browser functions
available in JavaScript, such as

- SetTimeout
- SetInterval
- SetImmediate
- ClearTimeout
- ClearInterval
- ClearImmediate

Original behaviour is closer to NodeJS
(Returns objects instead of numbers, has ability to keep application running,
et cetera) and is replicated as much as possible inside of the CLR

## Documentation

For the full guide on the package functionality, make sure to check out
[the docs][docs]

## Installation

### Package manager console

`Install-Package JsTimers`

### .NET CLI

`dotnet add package JsTimers`

## Usage

All methods of library's public API are located inside `JsTimers.TimerManager`
therefore, explanations below will not contain this type as prefix to presented method

For most cases it is easier to include `using static`
directive in your code as shown below:

```cs
using static JsTimers.TimerManager;
```

### Simple timer

To create basic timer, which will be fired once after specified delay,
use `SetTimeout` method:

```cs
SetTimeout(() => {
  Console.WriteLine("Hello, JsTimers!");
}, 1000);
```

This will issue a timeout of 1 second and then execute a given callback

### Cancelling timers

All methods mentioned above actually return objects which represent timers.
These objects could be assigned to a variable for further actions, such as **cancelling**:

```cs
Timeout timeout = SetTimeout(() => {
  Console.WriteLine("This callback will not fire");
}, 1000);
ClearTimeout(timeout);
```

## Important

Do not use this library to time execution of actions which require
very high precision. Library runs internal loop and processes all active timers
one by one, this might sometimes cause overhead of up to `30ms`,
therefore it works fine in most cases when you build general purpose software,
but if you want to build an atomic clock with that, I have bad news for you

[logo-github]: https://github.com/2chevskii/JsTimers/raw/master/assets/graphics/rendered/logo.png
[logo-local]: assets/graphics/rendered/logo.png
[icon-local]: assets/graphics/rendered/icon.png
[codefactor-badge]: https://img.shields.io/codefactor/grade/github/2chevskii/jstimers/master?color=%23f7df1e&logo=codefactor&logoColor=%23f7df1e&style=for-the-badge
[codefactor-stats]: https://www.codefactor.io/repository/github/2chevskii/jstimers
[appveyor-badge]: https://img.shields.io/appveyor/build/2chevskii/jstimers/master?color=%23f7df1e&logo=appveyor&logoColor=%23f7df1e&style=for-the-badge
[appveyor-build-status]: https://ci.appveyor.com/project/2chevskii/jstimers/branch/master
[nuget-badge]: https://img.shields.io/nuget/v/jstimers?color=%23f7df1e&logo=nuget&logoColor=%23f7df1e&style=for-the-badge
[nuget-package]: https://www.nuget.org/packages/JsTimers/
[docs]: https://2chevskii.github.io/JsTimers
