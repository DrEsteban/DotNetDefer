# DotNetDefer
A simple library to bring GoLang's `defer` functionality to .NET!

This version of DotNetDefer targets `netstandard2.0` and only implements `IDisposable` - for use with older projects.

## Usage

**Standard Actions:** (primary use-case)
```csharp
using DotNetDefer;

class Program
{
    static void Main(string[] args)
    {
        var process = new Process();
        process.Start();
        using var defer = new Defer(process.Kill);
    
        // Do some work
    }
}
```

**Use with `Func<T>`**
```csharp
using DotNetDefer;

class Program
{
    static void Main(string[] args)
    {
        var process = new Process();
        process.Start();
        var defer = new Defer(() => 
            {
                process.Kill();
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