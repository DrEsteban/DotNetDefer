# DotNetDefer
A simple library to bring GoLang's `defer` functionality to .NET!

* `DotNetDefer`
  * Implements `IDisposable` and `IAsyncDisposable`, and can be used with `Action`, `Func<T>` `Func<Task>` and `Func<Task<T>>`
* `DotNetDefer.Legacy`
  * Implements only `IDisposable` and can be used with `Action`s and `Func`s

## Usage
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

## Installation
```bash
dotnet add package DrEsteban.DotNetDefer

Install-Package DrEsteban.DotNetDefer
```

There's also a version of the package that targets `netstandard1.0` for compatibility with older projects. (E.g. projects that don't support `IAsyncDisposable`)
```bash
Install-Package DrEsteban.DotNetDefer.Legacy
```