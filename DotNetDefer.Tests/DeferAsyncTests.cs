using System.Reflection;

namespace DotNetDefer.Tests;
public class DeferAsyncTests
{
    [Fact]
    public async Task SimpleDefer()
    {
        bool b = false;
        DeferAsync defer;
        await using (defer = new DeferAsync(() =>
            {
                b = true;
                return Task.CompletedTask;
            }))
        {
            Assert.False(defer.IsDisposed);
            Assert.Null(defer.Error);
        }
        Assert.True(b);
        Assert.True(defer.IsDisposed);
    }

    [Fact]
    public void FinalizedDefer()
    {
        bool b = false;
        var defer = new DeferAsync(() =>
        {
            b = true;
            return Task.CompletedTask;
        });
        Assert.False(defer.IsDisposed);
        Assert.Null(defer.Error);

        var finalizer = typeof(DeferAsync).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(finalizer);
        finalizer.Invoke(defer, null);

        Assert.True(b);
        Assert.True(defer.IsDisposed);
    }

    [Fact]
    public async Task SimpleDeferFunc()
    {
        DeferAsync<string> defer;
        await using (defer = new DeferAsync<string>(() => Task.FromResult("string")))
        {
            Assert.False(defer.IsDisposed);
            Assert.Null(defer.Error);
            Assert.Null(defer.Result);
        }

        Assert.Equal("string", defer.Result);
        Assert.Null(defer.Error);
        Assert.True(defer.IsDisposed);
    }

    [Fact]
    public void FinalizedDeferFunc()
    {
        var defer = new DeferAsync<string>(() => Task.FromResult("string"));
        Assert.False(defer.IsDisposed);
        Assert.Null(defer.Error);
        Assert.Null(defer.Result);

        var finalizer = typeof(DeferAsync<string>).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(finalizer);
        finalizer.Invoke(defer, null);

        Assert.Equal("string", defer.Result);
        Assert.Null(defer.Error);
        Assert.True(defer.IsDisposed);
    }

    [Fact]
    public async Task ActionOnlyCalledOnce()
    {
        int x = 0;
        var defer = new DeferAsync(() =>
        {
            x++;
            return Task.CompletedTask;
        });
        await defer.DisposeAsync();
        await defer.DisposeAsync();
        Assert.Equal(1, x);
    }

    [Fact]
    public async Task SimpleDeferFuncWithException()
    {
        DeferAsync<string> defer = null;
        try
        {
            await using (defer = new DeferAsync<string>(() => throw new IndexOutOfRangeException("exception")))
            {
                Assert.False(defer.IsDisposed);
                Assert.Null(defer.Error);
                Assert.Null(defer.Result);
            }
            Assert.Fail("Expected exception");
        }
        catch (IndexOutOfRangeException e)
        {
            Assert.Equal("exception", e.Message);
        }

        Assert.NotNull(defer);
        Assert.Null(defer.Result);
        Assert.True(defer.IsDisposed);

        Assert.NotNull(defer.Error);
        Assert.IsType<IndexOutOfRangeException>(defer.Error);
        Assert.Equal("exception", defer.Error.Message);

        var error = defer.Error;
        try
        {
            await defer.DisposeAsync();
            Assert.Fail("Expected exception");
        }
        catch (IndexOutOfRangeException e)
        {
            Assert.Equal("exception", e.Message);
            Assert.Equal(error, e);
        }
    }
}
