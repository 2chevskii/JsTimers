![Project logo][logo-local]

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

### Repeating timer

`SetInterval` method is designed for creation of timers which repeteadly
execute their callbacks

```cs
int calls = 0;

SetInterval(() => {
  Console.WriteLine("This callback was executed {0} times", ++calls);
}, 1000);
```

> Both SetTimeout and SetInterval have overloads which accept floats
as second argument (instead of integers) and are considered seconds
(instead of milliseconds)

### Immediate timer

`SetImmediate` is a method which schedules a callback to be executed
on next internal timer tick, and to have higher priority than other timers.

<!-- markdownlint-disable MD013 -->
```cs
SetImmediate(() => {
  Console.WriteLine("This callback has higher priority than callbacks, scheduled with SetTimeout or SetInterval");
});
```
<!-- markdownlint-enable MD013 -->

### Cancelling timers

All methods mentioned above actually return objects which represent timers.
These objects could be assigned to a variable for further actions, such as **cancelling**:

```cs
Timeout timeout = SetTimeout(() => {
  Console.WriteLine("This callback will not fire");
}, 1000);
ClearTimeout(timeout);
```

Respective methods for other timer types are:

- `ClearInterval`
- `ClearImmediate`

### Refreshing timers

If you have a need to either restart destroyed timer or delay execution further,
you can call a `Refresh()` method on `Timeout` object

```cs
// Example 1:
var timeout = SetTimeout(() => {
  Console.WriteLine("I'm the callback")
}, 1000);
var timeout2 = SetTimeout(() => {
  timeout.Refresh();
}, 900);
// timeout will be executed in ~1900ms instead of 1000

// Example 2:
var timeout = SetTimeout(() => {
  Console.WriteLine("I will fire twice!");
}, 1000);
var timeout2 = SetTimeout(() => {
  timeout.Refresh();
  Console.WriteLine("Timeout 1 was resurrected!");
}, 1000);

```

### `Ref`/`UnRef` methods

All timers will by default prevent your application from exiting until destroyed,
but there is a way to explicitly tell them not to
<!-- markdownlint-disable MD013 -->
```cs
static void Main(string[] args) {
  var timeout = SetTimeout(() => {
    Console.WriteLine("Application won't close until my delay passes. Oh, wait...")
  }, 3000);
  timeout.UnRef(); // UnRef() method allows application to exit even if timer has not been destroyed yet
}
```
<!-- markdownlint-enable MD013 -->

If you want to restore previously disabled `Ref` on timer,
just call `Ref()` method again.
Don't forget though, that if timer has already been destroyed,
it will not prevent application exit, if only you do not `Refresh()` it
(Therefore you cannot `Ref` destroyed `Immediate`)

## Important

Do not use this library to time execution of actions which require
very high precision. Library runs internal loop and processes all active timers
one by one, this might sometimes cause overhead of up to `30ms`,
therefore it works fine in most cases when you build general purpose software,
but if you want to build an atomic clock with that, I have bad news for you

[logo-local]: assets/graphics/rendered/logo.png
[icon-local]: assets/graphics/rendered/icon.png
[codefactor-badge]: https://img.shields.io/codefactor/grade/github/2chevskii/jstimers/master?color=%23f7df1e&logo=codefactor&logoColor=%23f7df1e&style=for-the-badge
[codefactor-stats]: https://www.codefactor.io/repository/github/2chevskii/jstimers
[appveyor-badge]: https://img.shields.io/appveyor/build/2chevskii/jstimers/master?color=%23f7df1e&logo=appveyor&logoColor=%23f7df1e&style=for-the-badge
[appveyor-build-status]: https://ci.appveyor.com/project/2chevskii/jstimers/branch/master
[nuget-badge]: https://img.shields.io/nuget/v/jstimers?color=%23f7df1e&logo=nuget&logoColor=%23f7df1e&style=for-the-badge
[nuget-package]: https://www.nuget.org/packages/JsTimers/
