# CodeBrix.Platform.Extensions

A .NET Standard 2.0 and .NET 10 helper library for CodeBrix.Platform and its consumers — general extensions, collections, disposables, equality comparers, logging helpers, and threading primitives, all under the `CodeBrix.Platform.Extensions.*` namespaces.
CodeBrix.Platform.Extensions is provided as a .NET library and associated `CodeBrix.Platform.Extensions.ApacheLicenseForever` NuGet package.

CodeBrix.Platform.Extensions supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

The package also carries a .NET Standard 2.0 target. That target exists for one specific reason: Roslyn source generators and analyzers are loaded by the compiler and must be built against `netstandard2.0`, so this target lets a generator or analyzer project consume these helpers. It is not general downlevel support — for ordinary applications and libraries, target .NET 10.

## Installation

```
dotnet add package CodeBrix.Platform.Extensions.ApacheLicenseForever
```

Note that the NuGet package ID and the namespace are different - there is no package named plain `CodeBrix.Platform.Extensions`:

* NuGet package ID: `CodeBrix.Platform.Extensions.ApacheLicenseForever`
* Assembly and root namespace: `CodeBrix.Platform.Extensions` - i.e. `using CodeBrix.Platform.Extensions;`

Never write `using CodeBrix.Platform.Extensions.ApacheLicenseForever;` — that namespace does not exist. The suffix is a CodeBrix family convention that records the license the package will always be published under.

The package pulls in the following automatically; no version pinning is needed in the consuming project:

* `CodeBrix.ServiceLocator.MsplLicenseForever` - used only by the logging singleton. Note the spelling: the package id says ServiceLocat**or**, but the namespace it provides is `CodeBrix.ServiceLocation`.
* `Microsoft.Extensions.Logging` - the `ILogger` abstractions the logging helpers extend.
* On the .NET Standard 2.0 target only, the polyfill packages for what .NET 10 has in-box: `System.Collections.Immutable`, `System.Memory` and `System.Threading.Tasks.Extensions`.

## CodeBrix.Platform.Extensions supports:

* **General extensions** (`CodeBrix.Platform.Extensions`) — string/func/action/enum/stream/URI/date helpers, memoization, currying, async action helpers.
* **Collections** (`CodeBrix.Platform.Extensions.Collections`, `.Specialized`) — enumerable/list/dictionary/queue/stack extensions, observable-collection update helpers, weak attached dictionaries.
* **Disposables** (`CodeBrix.Platform.Extensions.Disposables`) — `CompositeDisposable`, `SerialDisposable`, `RefCountDisposable`, `AnonymousDisposable`, `NullDisposable`, and related helpers.
* **Equality** (`CodeBrix.Platform.Extensions.Equality`, `.Comparison`, `.Core.*`) — func/key/collection equality comparers and fast type comparison.
* **Logging** (`CodeBrix.Platform.Extensions.Logging`) — `ILogger`-based logging extension helpers and the singleton `LogExtensionPoint`.
* **Threading** (`CodeBrix.Platform.Extensions.Threading`) — `FastAsyncLock`, `FastTaskCompletionSource`, `AsyncEvent`, and `Transactional` immutable-update helpers.

## Sample Code

### Compose disposables

```csharp
using CodeBrix.Platform.Extensions.Disposables;

var subscriptions = new CompositeDisposable();
subscriptions.Add(Disposable.Create(() => Console.WriteLine("cleaned up")));
subscriptions.Dispose();
```

### Async lock

```csharp
using CodeBrix.Platform.Extensions.Threading;

var gate = new FastAsyncLock();
using (await gate.LockAsync(cancellationToken))
{
    // critical section
}
```

### Bootstrap the logging singleton

`LogExtensionPoint` and the `.Log()` extension methods live in the root `CodeBrix.Platform.Extensions` namespace. Assign the ambient factory once at startup, before anything calls `.Log()`:

```csharp
using CodeBrix.Platform.Extensions;   //LogExtensionPoint and the .Log() extensions
using Microsoft.Extensions.Logging;   //ILoggerFactory, ILogger

LogExtensionPoint.AmbientLoggerFactory = myLoggerFactory;   //any ILoggerFactory

//...afterwards, from any instance - or from a Type
this.Log().LogInformation("Ready.");
```

`.Log()` never throws for want of configuration: with no factory assigned it falls back to a `LoggerFactory` with no providers, so every message is silently dropped. "No output" is the failure mode, not an exception. The alternative bootstrap is to set a `CodeBrix.ServiceLocation` provider that can resolve `ILoggerFactory`; the direct assignment above needs no service locator at all. Either way, do it once — the per-type logger is cached the first time `.Log()` is called for that type.

## Documentation

The NuGet package includes `AGENT-README.txt`, a complete API reference and usage guide written for AI coding agents - point your agent at that file when it is writing code against this library. It is worth reading for one thing in particular: the folder layout inside the assembly does not match the namespaces, so the AGENT-README's namespace-by-namespace type listing is the fastest way to find a helper.

Additional sample code and usage examples are available in the `CodeBrix.Platform.Extensions.Tests` project:
https://github.com/ellisnet/CodeBrix.Platform.Extensions/tree/main/tests/CodeBrix.Platform.Extensions.Tests

## License

CodeBrix.Platform.Extensions is licensed under the Apache License, Version 2.0 - see the
[LICENSE](https://github.com/ellisnet/CodeBrix.Platform.Extensions/blob/main/LICENSE) file.
The package is published under the SPDX expression `Apache-2.0`.

For licensing and provenance information about the open source code included in
this package, see [THIRD-PARTY-NOTICES.txt](https://github.com/ellisnet/CodeBrix.Platform.Extensions/blob/main/THIRD-PARTY-NOTICES.txt).
