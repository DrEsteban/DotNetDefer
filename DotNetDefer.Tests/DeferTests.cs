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
}