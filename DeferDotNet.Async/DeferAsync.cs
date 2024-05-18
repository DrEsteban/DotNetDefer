using System;
using System.Threading.Tasks;
using DeferDotNet.Internal;

namespace DeferDotNet;

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
public sealed class DeferAsync(Func<Task> func, bool forceThread = false) : DeferAsyncBase(forceThread)
{
    protected override async Task ExecuteAsync()
    {
        await func().ConfigureAwait(false);
        func = null; // Allow the function to be garbage collected
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
public sealed class DeferAsync<T>(Func<Task<T>> func, bool forceThread = false) : DeferAsyncBase(forceThread), IDeferAsync<T>
{
    public T Result { get; private set; }

    protected override async Task ExecuteAsync()
    {
        this.Result = await func().ConfigureAwait(false);
        func = null; // Allow the function to be garbage collected
    }
}