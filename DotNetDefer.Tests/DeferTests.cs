using System.Reflection;

namespace DotNetDefer.Tests;

public class DeferTests
{
    [Fact]
    public void SimpleDefer()
    {
        bool b = false;
        Defer defer;
        using (defer = new Defer(() => b = true))
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
        var defer = new Defer(() => b = true);
        Assert.False(defer.IsDisposed);
        Assert.Null(defer.Error);

        var finalizer = typeof(Defer).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(finalizer);
        finalizer.Invoke(defer, null);

        Assert.True(b);
        Assert.True(defer.IsDisposed);
    }

    [Fact]
    public void SimpleDeferFunc() {
        Defer<string> defer;
        using (defer = new Defer<string>(() => "string"))
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
        var defer = new Defer<string>(() => "string");
        Assert.False(defer.IsDisposed);
        Assert.Null(defer.Error);
        Assert.Null(defer.Result);

        var finalizer = typeof(Defer<string>).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(finalizer);
        finalizer.Invoke(defer, null);

        Assert.Equal("string", defer.Result);
        Assert.Null(defer.Error);
        Assert.True(defer.IsDisposed);
    }

    [Fact]
    public void ActionOnlyCalledOnce()
    {
        int x = 0;
        var defer = new Defer(() => x++);
        defer.Dispose();
        defer.Dispose();
        Assert.Equal(1, x);
    }

    [Fact]
    public void SimpleDeferFuncWithException()
    {
        Defer<string> defer = null;
        try
        {
            using (defer = new Defer<string>(() => throw new IndexOutOfRangeException("exception")))
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
            defer.Dispose();
            Assert.Fail("Expected exception");
        }
        catch (IndexOutOfRangeException e)
        {
            Assert.Equal("exception", e.Message);
            Assert.Equal(error, e);
        }
    }
}