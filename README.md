# CodeBrix.Platform.Extensions

A .NET 10 bundle of the core nventive **Uno.Core.Extensions** helper libraries — general extensions, collections, disposables, equality comparers, logging helpers, and threading primitives — namespace-renamed into the `CodeBrix.Platform.Extensions.*` family for use by CodeBrix.Platform and its consumers.
CodeBrix.Platform.Extensions vendors the source of seven Uno.Core.Extensions packages at version 4.1.1 into a single assembly and `CodeBrix.Platform.Extensions.ApacheLicenseForever` NuGet package, so CodeBrix.Platform has one CodeBrix-owned package instead of a fan-out of nventive `Uno.Core.Extensions.*` references.

CodeBrix.Platform.Extensions supports applications and assemblies that target Microsoft .NET version 10.0 and later.
Microsoft .NET version 10.0 is a Long-Term Supported (LTS) version of .NET, and was released on Nov 11, 2025; and will be actively supported by Microsoft until Nov 14, 2028.
Please update your C#/.NET code and projects to the latest LTS version of Microsoft .NET.

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

## Provenance

This package is a namespace-renamed redistribution of nventive's Uno.Core.Extensions source at version 4.1.1 (commit `6da975c`). Namespaces are mapped `Uno.* → CodeBrix.Platform.Extensions.*` (e.g. `Uno.Disposables → CodeBrix.Platform.Extensions.Disposables`, `Uno.Extensions → CodeBrix.Platform.Extensions`). See `THIRD-PARTY-NOTICES.txt` for the full per-project provenance.

## License

The project is licensed under the Apache License, Version 2.0. see: https://en.wikipedia.org/wiki/Apache_License

The vendored helper source is itself Apache-2.0 (Copyright © nventive inc.); the combined package is published under the SPDX expression `Apache-2.0`.
