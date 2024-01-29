# Timer types

Library offers different types of timers, each serving it's purpose,
to make API *look like* browser's/nodejs' one, because it's... convenient?

## Timeout

### Full name

```cs
JsTimers.Timeout
```

### Description

Basic timer type, which does exactly what you think it does - simply delays
execution of a given function for a set amount of time

### Instantiation

```cs
using JsTimers;

static void Main() {
  Timeout timeout = TimerManager.SetTimeout(
    () => Console.WriteLine("This is the timeout's callback"),
    42
  );
}
```

## Interval

### Full name

```cs
JsTimers.Timeout
```

::: tip Confused? Ok.
No, this is not a mistake, the type is exactly the same,
only the internal flag is changed to make timer repeat it's
execution over and over again, until stopped explicitly
:::

### Description

This timer type will not stop after executed just once, function will be called with
the given interval indefinitely. To stop execution, you will have to manually
clear it.

### Instantiation

```cs
using JsTimers;

static void Main() {
  Timeout interval = TimerManager.SetInterval(
    () => Console.WriteLine("This is the interval's callback"),
    42
  );
}
```

## Immediate

### Full name

```cs
JsTimers.Immediate
```

### Description

This timer type is a bit different: it does not have the delay.
Due to the nature of library's internals, all timers have their own execution
queue. And they are called one after another, if their *next callback time*
is `<=` *current time*. But in case two timers' callbacks are scheduled
to be called at the same time, one of them will be delayed, until
the other is done. **Immediate** timers are always called first,
so this delay will be lower for them.

Since **Immediate** does not take a delay as an instantiation parameter,
it will be called immediately, when the next internal loop's cycle is
reached.

### Instantiation

```cs
using JsTimers;

static void Main() {
  Immediate immediate = TimerManager.SetImmediate(
    () => Console.WriteLine("This is the immediate's callback")
  );
}
```
