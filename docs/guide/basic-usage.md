# Basic usage

## Include package namespace

Inside file, where you plan to use package's functionality,
include the `using` directive:

```cs
using JsTimers;
```

::: tip Hint:
To reduce the amount of code you have to write every time you want to call
package methods, try the `using static` directive instead:

```cs
using static JsTimers.TimerManager;
```

:::

## Create your first timer

Instantiating a timer is as simple as calling the `TimerManager.SetTimeout` method:

```cs
TimerManager.SetTimeout(
  () => Console.WriteLine("Hello from JsTimers!"),
  1000
);
```

## Cancel the timer

Cancelling timers is also pretty straight-forward:

```cs
Timeout timer = TimerManager.SetTimeout(
  () => ThisWillNotBeCalled(),
  1000
);
TimerManager.ClearTimeout(timer);
```
