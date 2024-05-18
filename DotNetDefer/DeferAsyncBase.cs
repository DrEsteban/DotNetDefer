using System;
using System.Threading.Tasks;

namespace DotNetDefer.Internal;

public abstract class DeferAsyncBase(bool forceThread) : IDeferAsync
{
    ~DeferAsyncBase() => this.DisposeAsync(false).ConfigureAwait(false).GetAwaiter().GetResult();

    public bool IsDisposed { get; protected set; }
    public Exception Error { get; protected set; }

    public ValueTask DisposeAsync() => this.DisposeAsync(true);

    protected abstract Task ExecuteAsync();

    protected async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing && this.Error is not null)
        {
            throw this.Error;
        }

        if (!this.IsDisposed)
        {
            try
            {
                await (forceThread ? Task.Run(this.ExecuteAsync).ConfigureAwait(false) : this.ExecuteAsync().ConfigureAwait(false));
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