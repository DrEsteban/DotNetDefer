using System;

namespace DeferDotNet;

public interface IDefer : IDisposable
{
    public bool IsDisposed { get; }
    public Exception Error { get; }
}

public interface IDefer<out T> : IDefer
{
    public T Result { get; }
}

/// <summary>
/// Defers the execution of an action until the object is disposed.
/// </summary>
/// <example>
/// using var defer = new Defer(() => Console.WriteLine("Disposed"));
/// </example>
/// <remarks>
/// If the deferral fails, the exception will be thrown when the object is disposed.
/// The deferral will only be executed once if Dispose is called multiple times, but the exception will be thrown each time.
/// </remarks>
/// <param name="action">The action to defer.</param>
public sealed class Defer(Action action) : IDefer
{
    ~Defer() => this.Dispose();

    public bool IsDisposed { get; private set; }
    public Exception Error { get; private set; }

    public void Dispose()
    {
        if (this.Error is not null)
        {
            throw this.Error;
        }

        if (!this.IsDisposed)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                this.Error = e;
                throw;
            }
            finally
            {
                action = null;
                this.IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}

/// <summary>
/// Defers the execution of a function until the object is disposed.
/// </summary>
/// <example>
/// using (var defer = new Defer(() =>
///     {
///         process.Stop();
///         return process.ExitCode;
///     }))
/// {
/// 
/// }
/// ...
/// var exitCode = defer.Result;
/// </example>
/// <remarks>
/// If the deferral fails, the exception will be thrown when the object is disposed.
/// The deferral will only be executed once if Dispose is called multiple times, but the exception will be thrown each time.
/// </remarks>
/// <typeparam name="T">The type returned by the funciton.</typeparam>
/// <param name="func">The function to defer.</param>
public sealed class Defer<T>(Func<T> func) : IDefer<T>
{
    ~Defer() => this.Dispose();

    public T Result { get; private set; }
    public bool IsDisposed { get; private set; }
    public Exception Error { get; private set; }

    public void Dispose()
    {
        if (this.Error is not null)
        {
            throw this.Error;
        }

        if (!this.IsDisposed)
        {
            try
            {
                this.Result = func();
            }
            catch (Exception e)
            {
                this.Error = e;
                throw;
            }
            finally
            {
                func = null;
                this.IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}