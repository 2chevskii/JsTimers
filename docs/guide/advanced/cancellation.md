# Timer cancellation

All the created timers can be `cleared` (cancelled),
to stop their *subsequent* execution

Library offers methods, starting with `Clear` to serve this purpose

## ClearTimeout

### Syntax

```cs
void TimerManager.ClearTimeout(Timeout)
```

### Description

Cancells execution of the given timer.
Does nothing, if given timer has been already cancelled, or fired.

## ClearInterval

::: warning Warning
While technically, it is possible to use `ClearTimeout` on the *interval* timer,
and vice-versa, it is confusing and leads to less semantically correct code,
so don't. I'm serious.
:::

### Syntax

```cs
void TimerManager.ClearInterval(Timeout /* interval */)
```

### Description

Cancells all the subsequent executions of the given interval timer.
Does nothing, if given interval has been already cancelled.

## ClearImmediate

### Syntax

```cs
void TimerManager.ClearImmediate(Immediate)
```

### Description

Cancells execution of the given immediate timer.
Has no effect on cancelled/executed timers.
