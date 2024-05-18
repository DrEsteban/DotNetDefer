using System;

namespace DotNetDefer;

public interface IDefer : IDisposable
{
    public bool IsDisposed { get; }
    public Exception Error { get; }
}

public interface IDefer<out T> : IDefer
{
    public T Result { get; }
}