# Timer class

```cs
public abstract class Timer {...}
```

## Instance properties

### Id

```cs
public int Id { get; }
```

### Destroyed

```cs
public bool Destroyed { get; }
```

## Instance events

### OnError

```cs
public event Action<Exception> OnError;
```

## Instance methods

### Ref

```cs
public void Ref();
```

### UnRef

```cs
public void UnRef();
```

### HasRef()

```cs
public void HasRef();
```
