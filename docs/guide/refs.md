# Application exit prevention

Timers can (and will, by default) prevent application from exitting.
This chapter demonstrates, how to control this behaviour.

## Default behaviour

By default, all created timers have `Ref`, which means until the
timer is destroyed, application cannot exit (AppDomain.ProcessExit event has an
infinite loop inside, which will spin until the last timer with `Ref` has been destroyed)

## Examples

```cs
using JsTimers;

static void Main() {
  Timeout interval = TimerManager.SetInterval(() => {/* noop */}, 1000);
  bool refed = interval.HasRef(); // true
  // Application keeps running, because the timer is never destroyed
}

static void Main() {
  Timeout timeout = TimerManager.SetTimeout(() => {}, 1000);
  // Application keeps running for 1 second, until timeout execution
}

static void Main() {
  Timeout interval = TimerManager.SetInterval(() => {}, 1000);
  interval.UnRef();
  bool refed = interval.HasRef(); // false
  // Application exits, interval's callback never fires
}
```

## HasRef()

### Syntax

```cs
bool Timer.HasRef()
```

### Description

Indicates, whether the timer has an active `Ref` to it

## Ref()

### Syntax

```cs
void Timer.Ref()
```

### Description

Enables `Ref` on timer

## UnRef()

### Syntax

```cs
void Timer.UnRef()
```

### Description

Disables timer's `Ref`
