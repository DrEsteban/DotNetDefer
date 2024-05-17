using System;
using System.Threading.Tasks;

namespace DeferDotNet;

public interface IAsyncDefer : IAsyncDisposable
{
    public bool IsDisposed { get; }
    public Exception Error { get; }
}

public interface IAsyncDefer<out T> : IAsyncDefer
{
    public T Result { get; }
}

/// <summary>
/// Defers the execution of an asynchronous action until the object is disposed.
/// </summary>
/// <example>
/// await using var defer = new Defer(() => process.StopAsync());
/// await using var defer = new Defer(process.StopAsync);
/// </example>
/// <remarks>
/// If the deferral fails, the exception will be thrown when the object is disposed.
/// The deferral will only be executed once if Dispose is called multiple times, but the exception will be thrown each time.
/// </remarks>
/// <param name="func">The action to defer.</param>
/// <param name="forceThread">Whether to force the deferral to run on a separate thread using Task.Run().</param>
public sealed class Defer(Func<Task> func, bool forceThread = false) : IAsyncDefer
{
    ~Defer() => this.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    public bool IsDisposed { get; private set; }
    public Exception Error { get; private set; }

    public async ValueTask DisposeAsync()
    {
        if (this.Error is not null)
        {
            throw this.Error;
        }

        if (!this.IsDisposed)
        {
            try
            {
                await (forceThread ? Task.Run(func).ConfigureAwait(false) : func().ConfigureAwait(false));
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

/// <summary>
/// Defers the execution of an asynchronous function until the object is disposed.
/// </summary>
/// <example>
/// await using (var defer = new Defer(async () =>
///     {
///         await process.StopAsync();
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
/// <param name="forceThread">Whether to force the deferral to run on a separate thread using Task.Run().</param>
public sealed class Defer<T>(Func<Task<T>> func, bool forceThread = false) : IAsyncDefer<T>
{
    ~Defer() => this.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    public T Result { get; private set; }
    public bool IsDisposed { get; private set; }
    public Exception Error { get; private set; }

    public async ValueTask DisposeAsync()
    {
        if (this.Error is not null)
        {
            throw this.Error;
        }

        if (!this.IsDisposed)
        {
            try
            {
                this.Result = await (forceThread ? Task.Run(func).ConfigureAwait(false) : func().ConfigureAwait(false));
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