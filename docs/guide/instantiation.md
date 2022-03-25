# Timer instantiation

If you've read the previous chapter, you should already have a hint
of how to create timers, but this one will explain a bit more
on that topic.

## Basics

Library API offers 3 methods of timer instantiation, named starting with `Set`.

### Callback

All of them take the `System.Action` delegate as *first argument*.
This will be the timer's callback function, called when timer has
reached it's execution time milestone.

### Delay

Also, two of them take execution delay as *second argument*.
This delay can be either specified as `integer` value,
in which case library will treat it as **milliseconds**,
or you can pass a `float` value there, in which case
it will be treated as **seconds**.

### Result

Return type of each method is a object of type, derived from `JsTimers.Timer`

## SetTimeout

### Syntax

```cs
Timeout TimerManager.SetTimeout(Action, int)
// or
Timeout TimerManager.SetTimeout(Action, float)
```

### Description

Creates a `Timeout` object, set to fire *once* after the delay

## SetInterval

### Syntax

```cs
Timeout TimerManager.SetInterval(Action, int)
// or
Timeout TimerManager.SetInterval(Action, float)
```

### Description

Creates a `Timeout` object, set to fire *repeteatly*, each time delay passes

## SetImmediate

### Syntax

```cs
Immediate TimerManager.SetImmediate(Action)
```

### Description

Creates an `Immediate` object, which will fire *once* as soon as possible,
according to the inner execution queue
