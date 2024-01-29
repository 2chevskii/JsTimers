# TimerManager class

```cs
public static class TimerManager {...}
```

## Static events

### OnTimerError

```cs
public static event Action<Timer, Exception> OnTimerError;
```

## Static methods

### SetTimeout(Action, int)

```cs
public static Timeout SetTimeout(Action callback, int delay);
```

### SetTimeout(Action, float)

```cs
public static Timeout SetTimeout(Action callback, float delay);
```

### SetInterval(Action, int)

```cs
public static Timeout SetInterval(Action callback, int interval);
```

### SetInterval(Action, float)

```cs
public static Timeout SetInterval(Action callback, float interval);
```

### SetImmediate(Action)

```cs
public static Immediate SetImmediate(Action callback);
```

### ClearTimeout(Timeout)

```cs
public static void ClearTimeout(Timeout timeout);
```

### ClearInterval(Timeout)

```cs
public static void ClearInterval(Timeout timeout);
```

### ClearImmediate(Immediate)

```cs
public static void ClearImmediate(Immediate immediate);
```
