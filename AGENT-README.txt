========================================================================
AGENT-README: CodeBrix.Platform.Extensions
A Comprehensive Guide for AI Coding Agents
========================================================================


OVERVIEW
========================================================================

CodeBrix.Platform.Extensions is a .NET 10 bundle of the core nventive
Uno.Core.Extensions helper libraries, vendored from source at version
4.1.1 (commit 6da975c) and namespace-renamed into the
CodeBrix.Platform.Extensions.* family. It exists so CodeBrix.Platform can
depend on ONE CodeBrix-owned package instead of a fan-out of nventive
`Uno.Core.Extensions.*` NuGet references.

Seven upstream helper projects are merged into a single assembly:

  Uno.Core.Extensions               -> Extensions/        (26 files)
  Uno.Core.Extensions.Collections   -> Collections/       (14 files)
  Uno.Core.Extensions.Disposables   -> Disposables/       (13 files)
  Uno.Core.Extensions.Equality      -> Equality/          ( 8 files)
  Uno.Core.Extensions.Logging       -> Logging/           ( 2 files)
  Uno.Core.Extensions.Logging.Singleton -> LoggingSingleton/ (1 file)
  Uno.Core.Extensions.Threading     -> Threading/         ( 6 files)

The Uno.Core.Extensions.Compatibility package (174 files) is deliberately
NOT included: CodeBrix.Platform imports none of its exclusive namespaces.


INSTALLATION
========================================================================

NuGet package: CodeBrix.Platform.Extensions.ApacheLicenseForever

  dotnet add package CodeBrix.Platform.Extensions.ApacheLicenseForever

The library's root namespace is `CodeBrix.Platform.Extensions` (the
`.ApacheLicenseForever` suffix exists only on the NuGet PackageId for
license-disambiguation across the CodeBrix family).

Target framework: .NET 10.0 or higher.

NuGet dependencies: Microsoft.Extensions.Logging (used by the Logging
helpers) and CommonServiceLocator (used by LogExtensionPoint).
System.Collections.Immutable is provided in-box by the .NET 10 framework.


KEY NAMESPACES
========================================================================

Namespaces were renamed from upstream `Uno.*` to `CodeBrix.Platform.Extensions.*`:

  CodeBrix.Platform.Extensions               (was Uno, Uno.Extensions)
  CodeBrix.Platform.Extensions.Specialized   (was Uno.Extensions.Specialized)
  CodeBrix.Platform.Extensions.Collections   (was Uno.Collections)
  CodeBrix.Platform.Extensions.Disposables   (was Uno.Disposables)
  CodeBrix.Platform.Extensions.Equality      (was Uno.Equality)
  CodeBrix.Platform.Extensions.Comparison    (was Uno.Comparison)
  CodeBrix.Platform.Extensions.Core.Equality (was Uno.Core.Equality)
  CodeBrix.Platform.Extensions.Core.Comparison (was Uno.Core.Comparison)
  CodeBrix.Platform.Extensions.Logging       (was Uno.Logging)
  CodeBrix.Platform.Extensions.Threading     (was Uno.Threading)

Mapping rule: `Uno.Extensions` and the root `Uno` both collapse to
`CodeBrix.Platform.Extensions`; every other `Uno.X` becomes
`CodeBrix.Platform.Extensions.X`. Each renamed `namespace` line carries a
`// was previously: <original>` provenance comment.


CORE API REFERENCE (by area)
========================================================================

  Extensions/        ActionExtensions, FuncExtensions (+ memoize / async-retry),
                     StringExtensions, EnumExtensions, DoubleExtensions,
                     StreamExtensions, UriExtensions, WeakReferenceExtensions,
                     CachedTuple, DisposableAction, ActionAsync/FuncAsync.
  Collections/       EnumerableExtensions (+ GroupBy / Specialized),
                     ListExtensions, DictionaryExtensions, QueueExtensions,
                     StackExtensions, MemoryExtensions,
                     ObservableCollectionExtensions.Update, IUpdatable,
                     Weak/UnsafeWeakAttachedDictionary.
  Disposables/       Disposable (factory), CompositeDisposable, SerialDisposable,
                     RefCountDisposable, AnonymousDisposable,
                     CancellationDisposable, ConditionalDisposable,
                     DefaultDisposable, NullDisposable, ICancelable,
                     IExtensibleDisposable, DisposableExtensions.
  Equality/          FuncEqualityComparer, KeyEqualityComparer,
                     CollectionEqualityComparer, FuncComparer, FastTypeComparer,
                     IKeyEquatable, WeakReferenceEqualityComparer,
                     EqualityComparerExtensions.
  Logging/           LogExtensions (ILogger helpers, incl. conditional logging).
  LoggingSingleton/  LogExtensionPoint (CommonServiceLocator-based logger
                     factory resolver).
  Threading/         FastAsyncLock, FastTaskCompletionSource, AsyncEvent,
                     Transactional (ref-based immutable update helpers).


ARCHITECTURE
========================================================================

  CodeBrix.Platform.Extensions/
    src/CodeBrix.Platform.Extensions/
      CodeBrix.Platform.Extensions.csproj
      InternalsVisibleTo.cs
      Extensions/        Collections/   Disposables/   Equality/
      Logging/           LoggingSingleton/             Threading/
    tests/CodeBrix.Platform.Extensions.Tests/
    AGENT-README.txt  LICENSE (Apache-2.0)  README.md  THIRD-PARTY-NOTICES.txt

Each sub-folder corresponds to one upstream Uno.Core.Extensions project so
provenance is traceable. The assembly is a normal class library (unlike the
metadata-only CodeBrix.Platform.Fonts.* packages); it exposes the public
helper types in the renamed namespaces.


CODING CONVENTIONS (CodeBrix family) + PORT EXCEPTIONS
========================================================================

This is a faithful third-party vendored port, so two SITUATIONAL family
exceptions apply (the same ones used by CodeBrix.AssemblyTools and
CodeBrix.Platform.OpenGL) -- they apply ONLY to the vendored upstream code,
never to new CodeBrix code:

  * <GenerateDocumentationFile> is FALSE. The upstream sources have hundreds
    of public members with no XML doc comments (CS1591) plus some malformed
    upstream doc tags; retrofitting docs onto vendored third-party source is
    out of scope.
  * <NoWarn> includes CS8632 (stray nullable-reference annotations under a
    Nullable-disabled context) and CA2022, both originating in the upstream
    source. Nullable reference types remain OFF (no <Nullable>enable</>).

Otherwise the standard family rules hold: net10.0 only; no global usings;
fixed `Copyright (c) 2026 Jeremy Ellis and contributors. Portions Copyright
(c) nventive inc., licensed under Apache-2.0.`; tests use xUnit v3 +
SilverAssertions + coverlet.collector.

Upstream `#if` framework-selection directives are intentionally preserved
(not stripped); the net10 build compiles the live branches. Upstream
block-scoped namespaces are preserved for fidelity.


PROVENANCE
========================================================================

Every `.cs` file under src/ retains its original nventive Apache-2.0 header
and carries a `// was previously: Uno.<ns>` comment on its namespace line.
The one byte-level change to headers was normalizing an already-corrupted
copyright glyph (0xEF 0xBF 0xBD) to "(c)". Full per-project provenance,
the source commit, and the modification list are in THIRD-PARTY-NOTICES.txt.


TESTING
========================================================================

Tests live under tests/CodeBrix.Platform.Extensions.Tests/. Run with:

  dotnet test CodeBrix.Platform.Extensions.slnx

The suite is a structural / smoke suite (not a 1:1 port of upstream's unit
tests): it confirms the assembly loads and targets net10, that representative
public types from each area resolve in their renamed
`CodeBrix.Platform.Extensions.*` namespaces, and that core helpers
(CompositeDisposable, FastAsyncLock, the equality comparers, the enumerable
and string extensions) behave correctly.


KNOWN GOTCHAS
========================================================================

  * This is NOT a general-purpose drop-in for Uno.Core.Extensions: namespaces
    are renamed, so consuming code must use `CodeBrix.Platform.Extensions.*`,
    not `Uno.*`. It targets net10.0+ only.
  * Uno.Core.Extensions.Compatibility is not included. If a CodeBrix.Platform
    build ever needs a type that lived only in the Compatibility package, that
    specific type can be vendored in as a follow-up.
  * CodeBrix.Platform also embeds some Uno.Core.Extensions source under
    src/Uno.Foundation/Uno.Core.Extensions/. When wiring this package into the
    fork, watch for duplicate-type conflicts between that embedded copy and
    this package.
