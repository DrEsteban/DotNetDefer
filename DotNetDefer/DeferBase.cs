using System;

namespace DotNetDefer.Internal;

/// <summary>
/// Base class containing main logic. Not intended to be used directly.
/// </summary>
public abstract class DeferBase : IDefer
{
    ~DeferBase() => this.Dispose(false);

    public bool IsDisposed { get; protected set; }
    public Exception Error { get; protected set; }

    public void Dispose() => this.Dispose(true);

    protected abstract void Execute();

    protected void Dispose(bool disposing)
    {
        if (disposing && this.Error is not null)
        {
            throw this.Error;
        }

        if (!this.IsDisposed)
        {
            try
            {
                this.Execute();
            }
            catch (Exception e)
            {
                this.Error = e;
                if (disposing)
                {
                    throw;
                }
            }
            finally
            {
                this.IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}