# DeferDotNet
A simple library that attempts to bring GoLang's `defer` functionality to .NET

* `DeferDotNet`
  * Implements `IDisposable` and can be used with `Action`s and `Func`s
* `DeferDotNet.Async`
  * Implements `IAsyncDisposable` and can be used with `Func<Task>` and `Func<Task<T>>`

## Usage
```csharp
using DeferDotNet;

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

## Installation
TODO: Publish to NuGet

