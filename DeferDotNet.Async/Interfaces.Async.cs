﻿using System;

namespace DeferDotNet;

public interface IDeferAsync : IAsyncDisposable
{
    public bool IsDisposed { get; }
    public Exception Error { get; }
}

public interface IDeferAsync<out T> : IDeferAsync
{
    public T Result { get; }
}