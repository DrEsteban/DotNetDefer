# DotNetDefer
A simple library to bring GoLang's `defer` functionality to .NET!

## Usage

**Standard Actions:** (primary use-case)
```csharp
using DotNetDefer;

class Program
{
    static void Main(string[] args)
    {
        var process = new MyLongProcess();
        process.Start();
        using var defer = new Defer(() => process.Stop());
    
        // Do some work
    }
}
```

**Use with `Func<Task>`**
```csharp
using DotNetDefer;

class Program
{
	static async Task Main(string[] args)
	{
		var process = new MyLongAsyncProcess();
		await using var defer = new Defer(async () => await process.StopAsync());

        // Do some work
	}
}
```

**Use with lambdas that return a value**
```csharp
using DotNetDefer;

class Program
{
    static void Main(string[] args)
    {
        var process = new MyLongProcess();
        process.Start();
        var defer = new Defer(() => 
            {
                process.Stop();
                return process.ExitCode;
            });
        using (defer)
        {
          // Do some work
        }

        var exitCode = defer.Result;
    }
}
```