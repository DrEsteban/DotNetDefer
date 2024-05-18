using System;
using DotNetDefer.Internal;

namespace DotNetDefer;

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
public sealed class Defer(Action action) : DeferBase
{
    protected override void Execute()
    {
        action();
        action = null; // Allow the action to be garbage collected
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
public sealed class Defer<T>(Func<T> func) : DeferBase, IDefer<T>
{
    public T Result { get; private set; }

    protected override void Execute()
    {
        this.Result = func();
        func = null; // Allow the function to be garbage collected
    }
}