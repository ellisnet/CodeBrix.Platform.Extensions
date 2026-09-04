================================================================================
AGENT-README: CodeBrix.Platform.Extensions
A Guide for AI Coding Agents — CONSUMING the
CodeBrix.Platform.Extensions.ApacheLicenseForever NuGet package
================================================================================


OVERVIEW
========

CodeBrix.Platform.Extensions is a single-assembly bundle of general-purpose
.NET helper types: functional helpers (memoization, currying, retry), a
disposables toolkit, LINQ-style collection extensions, equality/comparison
comparers, ILogger convenience extensions, and lock-free threading primitives.
It has no UI dependency and no platform dependency — it is a plain class
library that any .NET project can reference.

It targets .NET 10 or later AND .NET Standard 2.0. The .NET Standard 2.0
target is not legacy support: Roslyn source generators and analyzers are
loaded by the compiler and must be built against netstandard2.0, so a
generator or analyzer that wants these helpers can reference this package.
Everything else should consume the .NET 10 target.

Provenance: this is a namespace-renamed vendored redistribution of the core
helper libraries of an upstream nventive open-source project. Seven upstream
helper projects were merged into one assembly. Every public type now lives
under CodeBrix.Platform.Extensions.* — the upstream namespaces are NOT present in
this assembly. Do not write using directives for the upstream namespaces;
they will not compile. Per-project provenance is in THIRD-PARTY-NOTICES.txt,
which ships inside the package.


INSTALLATION
============

  PackageId:  CodeBrix.Platform.Extensions.ApacheLicenseForever

    dotnet add package CodeBrix.Platform.Extensions.ApacheLicenseForever

The assembly and the root namespace are both CodeBrix.Platform.Extensions.
The ".ApacheLicenseForever" suffix exists only on the NuGet package id, to
disambiguate licensing across the CodeBrix family. Never write
"using CodeBrix.Platform.Extensions.ApacheLicenseForever;" — that namespace
does not exist.

Target frameworks:  net10.0  and  netstandard2.0

NuGet dependencies, by id:

  All targets
    CodeBrix.ServiceLocator.MsplLicenseForever
                                         (used only by LogExtensionPoint;
                                          namespace: CodeBrix.ServiceLocation)

  .NET 10 target
    Microsoft.Extensions.Logging
    (System.Collections.Immutable is in-box in the .NET 10 shared framework)

  .NET Standard 2.0 target only — polyfills for what net10 has in-box
    Microsoft.Extensions.Logging
    System.Collections.Immutable         (Transactional immutable helpers,
                                          FastTaskCompletionSource)
    System.Memory                        (Span/Memory helpers)
    System.Threading.Tasks.Extensions

License: Apache-2.0. The package requires license acceptance.

No native libraries, no OS restrictions, no MSBuild targets or props are
shipped. Referencing the package is the whole installation.


KEY NAMESPACES / USINGS
=======================

The folder layout inside the assembly does NOT match the namespaces. Use this
table, not the folder names, to pick your using directives.

  CodeBrix.Platform.Extensions            <- the big one; most helpers
      Actions, Actions<T>, Actions<T1,T2>, ActionExtensions,
      ActionAsync (5 delegate arities), ActionAsyncExtensions,
      FuncAsync (5 delegate arities), FuncExtensions, FuncMemoizeExtensions,
      Funcs, NullableFuncs<T>, CurryExtensions, MatchExtensions,
      CachedTuple (+ 2/3/4-arity generics), DisposableAction, NullDisposable,
      DoubleExtensions, EnumHelper, DateTimeUnit, StreamExtensions,
      StringExtensions, TextWriterExtensions, UriExtensions,
      WeakReferenceExtensions, LegacyAttribute, Null,
      CollectionExtensions, DictionaryExtensions, EnumerableExtensions,
      ListExtensions, QueueExtensions, StackExtensions,
      ObservableCollectionExtensions, ObservableCollectionUpdateResults<T>,
      IUpdatable<T>, IBindableGrouping<TKey,TItem>, BindableGroup<TKey,TItem>,
      GroupDescriptor<TKey,TItem>, GroupDescriptorComparer<TKey>,
      Transactional, LogExtensionPoint

  CodeBrix.Platform.Extensions.Collections
      MemoryExtensions, WeakAttachedDictionary<TOwner,TKey>,
      UnsafeWeakAttachedDictionary<TOwner,TKey>

  CodeBrix.Platform.Extensions.Specialized
      EnumerableExtensions (the non-generic IEnumerable overloads)

  CodeBrix.Platform.Extensions.Disposables
      Disposable, AnonymousDisposable, CompositeDisposable,
      CompositeDisposableExtensions, SerialDisposable, RefCountDisposable,
      CancellationDisposable, ConditionalDisposable, DefaultDisposable,
      DisposableExtensions, ICancelable, IExtensibleDisposable

  CodeBrix.Platform.Extensions.Equality
      FuncEqualityComparer, FuncEqualityComparer<T>,
      FuncEqualityComparer<T,TValue>, KeyEqualityComparer,
      KeyEqualityComparer<T>, CollectionEqualityComparer<T>,
      WeakReferenceEqualityComparer<T>, IKeyEquatable, IKeyEquatable<T>

  CodeBrix.Platform.Extensions.Comparison
      FuncComparer<T>

  CodeBrix.Platform.Extensions.Core.Equality
      EqualityComparerExtensions

  CodeBrix.Platform.Extensions.Core.Comparison
      FastTypeComparer

  CodeBrix.Platform.Extensions.Logging
      LogExtensions

  CodeBrix.Platform.Extensions.Threading
      FastAsyncLock, FastTaskCompletionSource<T> (+ nested TerminationType),
      AsyncEvent

Watch the three that surprise people: NullDisposable is in the ROOT namespace,
not .Disposables; the collection extension classes (EnumerableExtensions,
CollectionExtensions, ListExtensions, DictionaryExtensions, QueueExtensions,
StackExtensions, ObservableCollectionExtensions) are in the ROOT namespace,
not .Collections; and Transactional and LogExtensionPoint are in the ROOT
namespace too.


CORE API REFERENCE — THREADING
==============================

namespace CodeBrix.Platform.Extensions.Threading

  sealed class FastAsyncLock
      Task<IDisposable> LockAsync(CancellationToken ct)

      An async-aware mutex. Await it and dispose the returned handle to
      release. It is RE-ENTRANT within one ExecutionContext (it tracks the
      holder in an AsyncLocal), so a nested LockAsync on the same async flow
      completes synchronously instead of deadlocking. There is no
      constructor argument, no timeout overload and no synchronous Lock().

  class AsyncEvent
      AsyncEvent(int initialCount)          <- no parameterless constructor
      int CurrentCount { get; }
      Task<bool> Wait(CancellationToken cancellationToken)
      void Release()
      void Release(int releaseCount)

      A counting async gate (semaphore-shaped). Wait returns true when a
      count was consumed; it returns FALSE instead of throwing when the
      token is already cancelled.

  class FastTaskCompletionSource<T> : INotifyCompletion
      enum TerminationType { Running = 0, Result = 1, Exception = 2,
                             Canceled = 3 }
      TerminationType Termination { get; }
      bool IsCompleted { get; }
      bool IsCanceled { get; }
      T? Result { get; }
      Exception? Exception { get; }
      ExceptionDispatchInfo? ExceptionInfo { get; }
      void SetResult(T result)              bool TrySetResult(T result)
      void SetException(Exception e)        bool TrySetException(Exception e)
      void SetException(ExceptionDispatchInfo i)
      bool TrySetException(ExceptionDispatchInfo i)
      void SetCanceled()                    bool TrySetCanceled()
      T? GetResult()
      FastTaskCompletionSource<T> GetAwaiter()
      void OnCompleted(Action continuation)
      Task<T?> Task { get; }

      An allocation-light TaskCompletionSource replacement that is its own
      awaiter — you can `await source;` directly without touching .Task.

namespace CodeBrix.Platform.Extensions   (yes, the root namespace)

  static class Transactional
      Lock-free compare-and-swap update helpers built on Interlocked. The
      "ref" argument must be a field you own; the selector may run more than
      once when another thread wins the race, so it must be pure.

      T Update<T>(ref T original, Func<T,T> selector) where T : class?
      object Update(ref object original, Func<object,object> selector)
      T Update<T,TParam>(ref T original, TParam param,
                         Func<T,TParam,T> selector)
      T Update<T,TParam1,TParam2>(ref T original, TParam1 p1, TParam2 p2,
                                  Func<T,TParam1,TParam2,T> selector)
      TResult Update<TSource,TResult>(ref TSource original,
                         Func<TSource,Tuple<TSource,TResult>> selector)
      TResult Update<TSource,TParam,TResult>(ref TSource original,
                         TParam param,
                         Func<TSource,TParam,Tuple<TSource,TResult>> selector)

      Immutable-collection overloads (all take the collection by ref and are
      constrained to the matching System.Collections.Immutable interface):

        IImmutableDictionary<TKey,TValue>, constraint
        TDictionary : class, IImmutableDictionary<TKey,TValue>
          TValue GetOrAdd<TDictionary,TKey,TValue>(ref TDictionary dictionary,
                     TKey key, Func<TKey,TValue> factory)
          TValue GetOrAdd<TDictionary,TKey,TContext,TValue>(
                     ref TDictionary dictionary, TKey key, TContext context,
                     Func<TKey,TContext,TValue> factory)
          bool TryAdd<TDictionary,TKey,TValue>(ref TDictionary dictionary,
                     TKey key, Func<TKey,TValue> factory, out TValue value)
          bool TryRemove<TDictionary,TKey,TValue>(...)
          int Remove<TDictionary,TKey,TValue>(ref TDictionary list,
                     Func<KeyValuePair<TKey,TValue>,bool> removeSelector)
          TValue SetItem<TDictionary,TKey,TValue>(ref TDictionary dictionary,
                     TKey key, TValue value)
          TValue SetItem<TDictionary,TKey,TValue>(ref TDictionary dictionary,
                     TKey key, Func<TKey,TValue> factory)
          bool TryUpdateItem<TDictionary,TKey,TValue>(ref TDictionary d,
                     TKey key, TValue value)

        The UpdateItem / TryUpdateItem factory overloads are NOT generic over
        the dictionary type — each takes one concrete ref type, so the field
        you pass must be declared as exactly one of these three:
          TValue UpdateItem<TKey,TValue>(
                     ref IImmutableDictionary<TKey,TValue> dictionary,
                     TKey key, Func<TKey,TValue,TValue> factory)
          TValue UpdateItem<TKey,TValue>(
                     ref ImmutableDictionary<TKey,TValue> dictionary, ...)
          TValue UpdateItem<TKey,TValue>(
                     ref ImmutableSortedDictionary<TKey,TValue> dictionary,..)
          bool TryUpdateItem<TKey,TValue>(...)   same three ref shapes;
                     returns false when the key is absent

        IImmutableQueue<T>, constraint TQueue : class, IImmutableQueue<T>
          void Enqueue<TQueue,T>(ref TQueue queue, T value)
          T Enqueue<TQueue,T>(ref TQueue queue, Func<TQueue,T> valueFactory)
          bool TryDequeue<TQueue,T>(ref TQueue queue, out T value)
          T Dequeue<TQueue,T>(ref TQueue queue)

        IImmutableList<T>, constraint TList : class, IImmutableList<T>
          TList Add<TList,T>(ref TList list, T item)
          TList AddDistinct<TList,T>(ref TList list, T item)
          TList AddDistinct<TList,T>(ref TList list, T item,
                                     IEqualityComparer<T> comparer)
          bool TryAddDistinct<TList,T>(ref TList list, T item)
          bool TryAddDistinct<TList,T>(ref TList list, T item,
                                       IEqualityComparer<T> comparer)
          bool Remove<TList,T>(ref TList list, T item)
          int Remove<TList,T>(ref TList list, Func<T,bool> removeSelector)
          int RemoveRange<TList,T>(ref TList list, T[] items)
          int RemoveRange<TList,T>(ref TList list, T[] items,
                                   out IEnumerable<T> removedItems)


CORE API REFERENCE — DISPOSABLES
================================

namespace CodeBrix.Platform.Extensions.Disposables

  static class Disposable
      IDisposable Empty { get; }            <- returns DefaultDisposable.Instance
      IDisposable Create(Action dispose)    <- throws ArgumentNullException on
                                               a null action; the action runs
                                               AT MOST ONCE

  sealed class AnonymousDisposable : ICancelable
      AnonymousDisposable(Action dispose)
      bool IsDisposed { get; }
      void Dispose()
      (this is what Disposable.Create returns)

  sealed class CompositeDisposable : ICollection<IDisposable>, ICancelable
      CompositeDisposable()
      CompositeDisposable(int capacity)
      CompositeDisposable(params IDisposable[] disposables)
      CompositeDisposable(IEnumerable<IDisposable> disposables)
      int Count { get; }        bool IsReadOnly { get; }
      bool IsDisposed { get; }
      void Add(IDisposable item)            <- disposes item IMMEDIATELY if the
                                               composite is already disposed
      bool Remove(IDisposable item)         <- also disposes the removed item
      bool Contains(IDisposable item)
      void CopyTo(IDisposable[] array, int arrayIndex)
      void Clear()                          <- disposes the children, keeps
                                               the composite usable
      void Dispose()
      IEnumerator<IDisposable> GetEnumerator()

  static class CompositeDisposableExtensions
      CompositeDisposable Add(this CompositeDisposable disposable,
                              Action action)
      Wraps the action in Disposable.Create and returns the same composite,
      so calls chain.

  sealed class SerialDisposable : ICancelable
      IDisposable? Disposable { get; set; }  <- assigning disposes the previous
                                                value; assigning after the
                                                SerialDisposable is disposed
                                                disposes the new value at once
      bool IsDisposed { get; }               void Dispose()

  sealed class RefCountDisposable : ICancelable
      RefCountDisposable(IDisposable disposable)
      RefCountDisposable(IDisposable disposable, bool throwWhenDisposed)
      IDisposable GetDisposable()            <- one reference; the underlying
                                                disposable is disposed when
                                                Dispose() has been called AND
                                                all handed-out references are
                                                disposed
      bool IsDisposed { get; }               void Dispose()

  sealed class CancellationDisposable : ICancelable
      CancellationDisposable()
      CancellationDisposable(CancellationTokenSource cts)
      CancellationToken Token { get; }       <- Dispose() cancels the source
      bool IsDisposed { get; }               void Dispose()

  class ConditionalDisposable : IDisposable
      ConditionalDisposable(object target, Action action,
                            WeakReference? conditionSource = null)
      void Dispose()
      Keeps itself alive only as long as `target` is alive (via a
      ConditionalWeakTable), so the action runs when target is collected or
      when Dispose() is called. `conditionSource`, when supplied and already
      collected, suppresses the action.

  sealed class DefaultDisposable : IDisposable
      static readonly DefaultDisposable Instance
      A no-op disposable singleton.

  interface ICancelable : IDisposable
      bool IsDisposed { get; }

  interface IExtensibleDisposable : IDisposable
      IReadOnlyCollection<object> Extensions { get; }
      IDisposable RegisterExtension<T>(T extension)
          where T : class, IDisposable

  static class DisposableExtensions
      T? DisposeWith<T>(this T? disposable, ICollection<IDisposable> composite)
      T DisposeWith<T>(this T disposable, SerialDisposable serialDisposable)
      void SafeDispose(this IDisposable disposable)
      bool TryDispose(this object maybeDisposableObject)
      void DisposeAll<T>(this IEnumerable<T> source)

namespace CodeBrix.Platform.Extensions   (root namespace — note the location)

  class NullDisposable : IDisposable
      static readonly IDisposable Instance
      void Dispose()

  class DisposableAction : IDisposable
      DisposableAction(Action action)
      Action Action { get; }
      void Dispose()


CORE API REFERENCE — LOGGING
============================

namespace CodeBrix.Platform.Extensions   (root namespace)

  static class LogExtensionPoint
      static ILoggerFactory AmbientLoggerFactory { get; set; }
      static ILogger Log(this Type forType)
      static ILogger Log<T>(this T instance)

HOW LOGGING IS BOOTSTRAPPED — verified against the source, read this before
you call .Log():

  * .Log() never throws for want of configuration. If nothing is registered
    it silently falls back to `new LoggerFactory()` — a factory with no
    providers, which drops every message. "No output" is the failure mode,
    not an exception.
  * AmbientLoggerFactory resolves lazily on FIRST use, in this order:
      1. If you assigned AmbientLoggerFactory yourself, that instance is used.
      2. Else, if CodeBrix.ServiceLocation's ServiceLocator.IsLocationProviderSet
         is true, it calls ServiceLocator.Current.GetService(typeof(
         ILoggerFactory)). A resolved ILoggerFactory is used. A resolved
         object of the wrong type raises InvalidOperationException.
      3. Else (or when the locator throws NullReferenceException /
         InvalidOperationException) it falls back to an empty LoggerFactory.
  * So there are exactly two supported bootstraps: assign
    LogExtensionPoint.AmbientLoggerFactory = <your factory>, or set a
    CodeBrix.ServiceLocation provider that can resolve ILoggerFactory. The
    direct assignment is the simpler one and needs no ServiceLocator at all.

  * BREAKING CHANGE — the locator moved. This package previously used the
    CommonServiceLocator package; it now uses
    CodeBrix.ServiceLocator.MsplLicenseForever, an API-identical port of
    CommonServiceLocator. Note the spelling: the PackageId says
    ServiceLocatOR but the NAMESPACE you import is CodeBrix.ServiceLocatION.

        using CodeBrix.ServiceLocation;   // then: ServiceLocator.SetLocatorProvider(...)

    The ambient provider is a private static field PER ASSEMBLY, so the two
    do not see each other. If you bootstrap logging by calling
    CommonServiceLocator's ServiceLocator.SetLocatorProvider(...),
    LogExtensionPoint will no longer observe it: IsLocationProviderSet reads
    false and you silently get an empty LoggerFactory (no exception, no
    output). Switch that call to CodeBrix.ServiceLocation's ServiceLocator,
    or use the direct AmbientLoggerFactory assignment instead.

  * The namespace is `CodeBrix.ServiceLocation`, NOT `CodeBrix.ServiceLocator`.
    `using CodeBrix.ServiceLocator;` does not compile. The locator package used
    the `CodeBrix.ServiceLocator` namespace in its earlier releases and renamed
    it, precisely because a namespace of that name is a member of the
    enclosing `CodeBrix` namespace and hid the `ServiceLocator` CLASS from every
    `CodeBrix.*` consumer — this very file hit that. With the rename, registering
    a provider needs no alias or qualification from any namespace:

        namespace CodeBrix.MyApp
        {
            using CodeBrix.ServiceLocation;
            // ... ServiceLocator.SetLocatorProvider(() => myAdapter);
        }
  * Do the bootstrap ONCE at startup, before anything calls .Log(). The
    per-type logger is cached in a static generic field the first time
    `instance.Log()` is called for that T; replacing the factory afterwards
    does not re-create loggers that were already handed out.
  * Log<T>(this T instance) keys the logger off the STATIC type T, not the
    runtime type. `object o = new Repository(); o.Log()` yields a logger
    named for System.Object. Use `this.Log()` from inside the class, or
    `typeof(Repository).Log()`, to get the name you expect.

namespace CodeBrix.Platform.Extensions.Logging

  static class LogExtensions — extension methods on Microsoft.Extensions.
  Logging.ILogger, in five families. Every method is `this ILogger log`.

      void LogFormat(this ILogger log, LogLevel level, object message)
      void LogFormat(this ILogger log, LogLevel level, string format,
                     object arg0 [, object arg1 [, object arg2]])
      void LogFormat(this ILogger log, LogLevel level, string format,
                     params object[] args)
      void Log(this ILogger log, LogLevel level, string message,
               Exception? exception = null)
      void Log(this ILogger log, LogLevel level, Func<object> messageBuilder,
               Exception? exception = null)

      For each of Trace / Debug / Info / Warn / Error / Critical there is the
      same trio:
        void <Name>Format(this ILogger log, object message)
        void <Name>Format(this ILogger log, string format, object arg0
                          [, object arg1 [, object arg2]])
        void <Name>Format(this ILogger log, string format,
                          params object[] args)
        void <Name>(this ILogger log, string message,
                    Exception? exception = null)
        void <Name>(this ILogger log, Func<object> messageBuilder,
                    Exception? exception = null)
      i.e. Trace/TraceFormat, Debug/DebugFormat, Info/InfoFormat,
      Warn/WarnFormat, Error/ErrorFormat, Critical/CriticalFormat.
      Note the names Info / Warn — not Information / Warning.

      Conditional variants that check IsEnabled before building the message:
        void DebugIfEnabled(this ILogger log, Func<string> messageSelector,
                            Exception? error = null)
        void InfoIfEnabled(...)     void WarnIfEnabled(...)
        void ErrorIfEnabled(...)    void CriticalIfEnabled(...)


CORE API REFERENCE — FUNCTIONAL HELPERS
=======================================

namespace CodeBrix.Platform.Extensions

  static class FuncMemoizeExtensions
      Non-thread-safe memoizers (a plain Dictionary, no lock — fastest, use
      when the caller is single-threaded or externally synchronized):
        Func<T> AsMemoized<T>(this Func<T> func)
        Func<TParam,TResult> AsMemoized<TParam,TResult>(...)
        ... arities up to 5 parameters ...
        Func<CancellationToken,Task<T>> AsMemoized<T>(
            this Func<CancellationToken,Task<T>> func)
        FuncAsync<T> AsMemoized<T>(this FuncAsync<T> func)
        FuncAsync<TParam,TResult> AsMemoized<TParam,TResult>(
            this FuncAsync<TParam,TResult> func)
      Thread-safe memoizers (double-checked lock / concurrent dictionary):
        Func<T> AsLockedMemoized<T>(this Func<T> func)
        Func<TKey,TResult> AsLockedMemoized<TKey,TResult>(...)
        Func<TArg1,TArg2,TResult> AsLockedMemoized<...>(...)
        Func<TArg1,TArg2,TArg3,TResult> AsLockedMemoized<...>(...)
        Func<TArg1,TArg2,TArg3,TArg4,TResult> AsLockedMemoized<...>(...)

  static class FuncExtensions
      Retry (the returned Task fails with the LAST exception once the
      attempt budget is exhausted; `tries` is the TOTAL number of attempts,
      not the number of retries; the default delay between attempts is
      100 ms):
        Task<T> Retry<T>(this Func<Task<T>> selector, int tries = 3,
                         TimeSpan? retryDelay = null)
        Task<T> Retry<T>(this Func<CancellationToken,Task<T>> selector,
                         CancellationToken ct, int tries = 3,
                         TimeSpan? retryDelay = null)
        Task Retry(this Func<Task> action, int tries = 3,
                   TimeSpan? retryDelay = null)
        Task Retry(this Func<CancellationToken,Task> action,
                   CancellationToken ct, int tries = 3,
                   TimeSpan? retryDelay = null)
      Weak-reference memoization keyed on an instance (the cache does not
      keep `source` alive):
        TResult ApplyMemoized<TSource,TResult>(this TSource source,
                         Func<TSource,TResult> selector)
        TResult ApplyMemoized<TSource,TResult,TParam>(this TSource source,
                         Func<TSource,TParam,TResult> selector, TParam param)
        TResult ApplyMemoized<TSource,TResult,TParam1,TParam2>(...)
        Func<TResult> AsWeakMemoized<TSource,TResult>(
                         this Func<TSource,TResult> selector, TSource source)
        Func<TParam,TResult> AsWeakMemoized<TSource,TResult,TParam>(...)
        Func<TParam1,TParam2,TResult> AsWeakMemoized<...>(...)
      Shape conversions:
        Func<T,bool> Not<T>(this Func<T,bool> func)
        Func<T> ToFunc<T>(this Func<Null,T> func)
        Func<Null,T> ToFunc<T>(this Func<T> func)
        Func<T,Null> ToFunc<T>(this Action<T> action)
        Func<Null,Null> ToFunc(this Action action)
        Func<T,T> ToFunc<T>(this Action action)
        Func<U> ToFunc<T,U>(this Func<T> func)
        Func<T,T> ToInterceptor<T>(this Action<T> action)
        Action<TRequest> ToAction<TRequest,TResponse>(
                         this Func<TRequest,TResponse> func)
        Action ToAction<TResponse>(this Func<Null,TResponse> func)
        Action<TKey,TValue> ToAction<TKey,TValue>(
                         this Action<KeyValuePair<TKey,TValue>> action)

  static class Funcs — factories that give the compiler a target type for a
  lambda, plus memoizing factories.
        Func<TResult> Create<TResult>(Func<TResult> function)
        Func<TParam,TResult> Create<TParam,TResult>(...)      (up to 3 params)
        FuncAsync<TResult> CreateAsync<TResult>(FuncAsync<TResult> function)
        FuncAsync<...> CreateAsync<...>(...)                  (up to 3 params)
        Func<CancellationToken,Task<TResult>> Create<TResult>(...)
        Func<CancellationToken,TParam,Task<TResult>> Create<TParam,TResult>(..)
        Func<TResult> CreateMemoized<TResult>(Func<TResult> function)
        Func<...> CreateMemoized<...>(...)                    (up to 5 params)
        Func<TResult> CreateLockedMemoized<TResult>(Func<TResult> function)
        FuncAsync<TResult> CreateAsyncMemoized<TResult>(...)
        FuncAsync<TParam,TResult> CreateAsyncMemoized<TParam,TResult>(...)
        Func<CancellationToken,Task<TResult>> CreateMemoized<TResult>(...)
        Func<CancellationToken,TParam,Task<TResult>> CreateMemoized<...>(...)

  static class NullableFuncs<T> where T : struct
        static readonly Func<T?,T> FromNullable   (GetValueOrDefault)
        static readonly Func<T,T?> ToNullable

  static class CurryExtensions — partial application. Four families (Func,
  FuncAsync, Action, ActionAsync), each with:
        Curry<T,TResult>(this Func<T,TResult> func, T value)
            -> Func<TResult>
        CurryFirst<T1,T2,TResult>(this Func<T1,T2,TResult> func, T1 first)
            -> Func<T2,TResult>
        CurryLast<T1,T2,TResult>(this Func<T1,T2,TResult> func, T2 last)
            -> Func<T1,TResult>
        CurryFirst / CurryLast also exist for the 3-parameter and
        4-parameter overloads, peeling one argument off the front or back.

  static class Actions
        static readonly Action Null                 (a no-op action)
        static readonly ActionAsync NullAsync
        Action Create(Action action)
        Action<T> Create<T>(Action<T> action)
        ActionAsync CreateAsync(ActionAsync action)
        ActionAsync<T> CreateAsync<T>(ActionAsync<T> action)
        Action CreateOnce(Action action)  <- thread-safe run-at-most-once
                                             wrapper (Interlocked.Exchange)
        IDisposable ToDisposable(Action action)

  static class Actions<T> / Actions<T1,T2>
        static readonly Action<T> Null / Action<T1,T2> Null
        static readonly ActionAsync<T> NullAsync / ActionAsync<T1,T2> NullAsync

  static class ActionExtensions
        IDisposable ToDisposable(this Action action)

  static class ActionAsyncExtensions — null-tolerant invocation
        Task SafeInvoke(this ActionAsync action, CancellationToken ct)
        Task SafeInvoke<TParam>(this ActionAsync<TParam> action,
                                CancellationToken ct, TParam param)

  delegates (cancellation token FIRST, by convention)
        delegate Task ActionAsync(CancellationToken ct)
        delegate Task ActionAsync<in T1>(CancellationToken ct, T1 value)
        delegate Task ActionAsync<in T1, in T2>(CancellationToken ct,
                                                T1 t1, T2 t2)
        delegate Task ActionAsync<in T1, in T2, in T3>(...)
        delegate Task ActionAsync<in T1, in T2, in T3, in T4>(...)
        delegate Task<TResult> FuncAsync<TResult>(CancellationToken ct)
        delegate Task<TResult> FuncAsync<in T1, TResult>(CancellationToken ct,
                                                         T1 t1)
        delegate Task<TResult> FuncAsync<in T1, in T2, TResult>(...)
        delegate Task<TResult> FuncAsync<in T1, in T2, in T3, TResult>(...)
        delegate Task<TResult> FuncAsync<in T1, in T2, in T3, in T4, TResult>(..)

  class Null — a sealed-by-convention placeholder for "void" in generic
  signatures. It has only a private constructor: you can never create one,
  you only ever pass null for it.


CORE API REFERENCE — COLLECTIONS
================================

namespace CodeBrix.Platform.Extensions   (root namespace)

  static class EnumerableExtensions — highlights (all extension methods on
  IEnumerable<T> unless noted):
        IEnumerable<T> ForEach<T>(this IEnumerable<T> items,
                                  Action<T> action)          <- EAGER, and it
                                     returns the source so you can keep going
        IEnumerable<T> ForEach<T>(this IEnumerable<T> items,
                                  Action<int,T> action)      <- index + item
        IEnumerable<T> ForEach<T>(this IEnumerable<T> items) <- drains it
        IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
                                     <- LAZY counterpart of ForEach
        bool None<T>(...)                bool None<T>(..., Func<T,bool> pred)
        bool Empty<T>(this IEnumerable<T> items)
        IEnumerable<T> Safe<T>(this IEnumerable<T> items)  <- null -> empty
        IEnumerable<T> Concat<T>(this IEnumerable<T> items, T item)
        IEnumerable<T> PrependEx<T>(this IEnumerable<T> items, T item)
        IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] items)
        IEnumerable<T> Except<T>(this IEnumerable<T> source,
                                 IEqualityComparer<T> cmp, params T[] items)
        int IndexOf<T>(this IEnumerable<T> items, T item)
        int IndexOf<T>(..., IEqualityComparer<T> comparer)
        int IndexOf<T>(..., Func<T,T,bool> predicate)
        bool AreDistinct<T>(...)         bool AreDistinct<T>(..., comparer)
        TResult SingleOrDefault<T,TResult>(this IEnumerable<T> items,
                                           Func<T,TResult> selector)
        T MinOrDefault<T>(...)           T MaxOrDefault<T>(...)
        TResult MaxOrDefault<TSource,TResult>(IEnumerable<TSource> source,
                     Func<TSource,TResult> selector,
                     TResult defaultValue = default)         <- NOT an
                                     extension method; call it statically
        (TSource Item, TComparable Value) MinBy<TSource,TComparable>(
                     this IEnumerable<TSource> source,
                     Func<TSource,TComparable> selector)
        (TSource Item, TComparable Value) MaxBy<TSource,TComparable>(...)
        IEnumerable<T> Range<T>(this IEnumerable<T> collection, int start,
                     int before, int after, bool fixedCount = true)
        IEnumerable<T> Trim<T>(this IEnumerable<T> items) where T : class
        IEnumerable<T> Trim<T>(this IEnumerable<T?> items) where T : struct
        ObservableCollection<T> ToObservableCollection<T>(...)
        double StdDev(this IEnumerable<double> values)
        IEnumerable<T> Flatten<T>(this IEnumerable<T> enumerable,
                                  Func<T,IEnumerable<T>> predicate)
        IEnumerable<T> Flatten<T>(this T item, Func<T,IEnumerable<T>> pred)
        IEnumerable<T> Flatten<T>(this T item, Func<T,T> predicate)
        bool AllEquals<T>(this IEnumerable<T> items,
                          IEqualityComparer<T> comparer = null)
        bool AllTrue(this IEnumerable<bool> source)
        bool AllTrueOrDefault(this IEnumerable<bool> source, bool defaultValue)
        bool AnyTrue(this IEnumerable<bool> source)
        bool AnyTrueOrDefault(this IEnumerable<bool> source, bool defaultValue)
        IEnumerable<T> GetPage<T>(this IEnumerable<T> source, int page,
                                  int perPage)
        bool SafeSequenceEqual<T>(this IEnumerable<T> obj,
                                  IEnumerable<T> other)
        long ConsecutiveValueCount<T>(this IEnumerable<T> source)
        Dictionary<TKey,TValue> ToDictionaryDistinct<...>(...) (4 overloads)
        Dictionary<TKey,IEnumerable<TValue>> ToDictionary<TKey,TValue>(
                     this IEnumerable<IGrouping<TKey,TValue>> groups
                     [, IEqualityComparer<TKey> equalityComparer])
        IEnumerable<TResult> FullOuterJoin<T1,T2,TKey,TResult>(...)
        IEnumerable SelectManyUntyped<TSource>(this IEnumerable<TSource> src,
                     Func<TSource,IEnumerable> selector)
        IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)

  static class CollectionExtensions — on ICollection<T> / arrays / List<T>
        T AddNew<T>(this ICollection<T> items)
        void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        IDisposable DisposableAdd<T>(this ICollection<T> collection, T item)
        IDisposable Subscribe<T>(this ICollection<T> collection, T item)
                     (both add the item and remove it again on Dispose)
        int Remove<T>(this ICollection<T> collection, Func<T,bool> predicate)
        void ReplaceWith<T>(this ICollection<T> collection,
                            IEnumerable<T> items)
        bool AddDistinct<T>(this ICollection<T> collection, T item)
        bool AddDistinct<T>(..., IEqualityComparer<T> comparer)
        bool AddDistinct<T>(..., Func<T,T,bool> predicate)
        int AddRangeDistinct<T>(...)                          (3 overloads)
        T FindOrCreate<T>(this ICollection<T> collection,
                          Func<T,bool> predicate, Func<T> factory)
        TResult[] SelectToArray<TSource,TResult>(...)         (3 overloads)
        TSource[] ToRangeArray<TSource>(this TSource[] source, int skip,
                                        int take)
        List<TResult> SelectToList<TSource,TResult>(...)      (4 overloads)
        List<TSource> WhereToList<TSource>(this List<TSource> source,
                                           Func<TSource,bool> selector)
        List<TSource> ToRangeList<TSource>(this List<TSource> source,
                                           int skip, int take)
      The SelectToArray / SelectToList family exists to avoid the iterator
      allocations of .Select().ToArray() when the source count is known.

  static class ListExtensions
        IList<T> AsReadOnly<T>(this IList<T> items)
        void AddRange(this IList destination, IEnumerable source)
        void AddOrReplaceRange<T>(this IList<T> list, IEnumerable<T> items)
        void AddOrReplaceRange<T>(this IList<T> list, IEnumerable<T> items,
                                  Func<T,T,bool> predicate)
        void RemoveAllAt<T>(this List<T> list, int index)  <- removes index
                                                              and everything
                                                              after it
        int Replace<T>(this IList<T> list, Func<T,bool> selector,
                       T replacement)
        IEnumerable<T> ToDivergentEnumerable<T>(this IList<T> list,
                       int startingAt)                     (also IReadOnlyList)
        bool ContainsIndex<T>(this IList<T> list, int index)
                                                           (also IReadOnlyList)
        T FindNearestItem<T>(this IList<T> list, Func<T,bool> predicate,
                             int startingAt = 0)           (also IReadOnlyList)
        int IndexOf(this IList list, object value, IEqualityComparer comparer)
        bool SequenceKeyEqual<T>(this IList<T> first, IList<T> second)

  static class DictionaryExtensions
        TValue FindOrCreate<TKey,TValue>(this IDictionary<TKey,TValue> items,
                       TKey key, Func<TValue> factory)
        TValue PlatformGetValueOrDefault<TKey,TValue>(
                       this IDictionary<TKey,TValue> dictionary, TKey key)
        TValue PlatformGetValueOrDefault<TKey,TValue>(..., TValue defaultValue)
        IEnumerable<TKey> RemoveKeys<TKey,TValue>(
                       this IDictionary<TKey,TValue> items,
                       IEnumerable<TKey> range)
        void Merge<TKey,TValue>(this IDictionary<TKey,TValue> dictionnary,
                       IEnumerable<KeyValuePair<TKey,TValue>> items)
        TValue GetValueOrDefaultAndRemove<TKey,TValue>(
                       this IDictionary<TKey,TValue> items, TKey key)
      The "Platform" prefix on PlatformGetValueOrDefault is deliberate: it
      keeps these from colliding with the BCL's GetValueOrDefault.

  static class QueueExtensions
        bool Remove<T>(this Queue<T> queue, Func<T,bool> predicate)
        T DequeueOrDefault<T>(this Queue<T> queue)

  static class StackExtensions
        T PeekOrDefault<T>(this Stack<T> stack)
        IDisposable Subscribe<T>(this Stack<T> stack, T value)
                     (pushes now, pops on Dispose)

  static class ObservableCollectionExtensions — differential update: patch a
  bound collection in place instead of Clear()+AddRange(), so bindings and
  selection survive.
        void Update<T>(this IList<T> collection, IEnumerable<T> updated,
                       bool tryDispose = false,
                       IEqualityComparer<T> comparer = null)
        ObservableCollectionUpdateResults<T> UpdateWithResults<T>(
                       this IList<T> collection, IEnumerable<T> updated,
                       bool tryDispose = false,
                       IEqualityComparer<T> comparer = null)
        Task UpdateAsync<T>(this IList<T> collection, CancellationToken ct,
                       IEnumerable<T> updated, bool tryDispose = false,
                       IEqualityComparer<T> comparer = null)
      Pass tryDispose: true ONLY when the incoming items are different
      instances matched by Equals — matching items are not compared by
      reference, so with shared instances you would dispose live objects.
      UpdateAsync additionally calls IUpdatable<T>.UpdateAsync on every kept
      item, letting an existing item absorb the newer instance's values.

  class ObservableCollectionUpdateResults<T>
        ObservableCollectionUpdateResults(IEnumerable<T> added,
                       IEnumerable<T> moved, IEnumerable<T> removed)
        IEnumerable<T> Added { get; }
        IEnumerable<T> Moved { get; }
        IEnumerable<T> Removed { get; }

  interface IUpdatable<T>
        Task UpdateAsync(CancellationToken ct, T newerInstance)

namespace CodeBrix.Platform.Extensions.Specialized

  static class EnumerableExtensions — the same ideas for the NON-generic
  System.Collections.IEnumerable, useful when you only have `object`:
        int Count(this IEnumerable enumerable)
        bool Any(this IEnumerable items)
        bool None(this IEnumerable source)
        object[] ToObjectArray(this IEnumerable items)
        int IndexOf(this IEnumerable items, object item)
        object ElementAt(this IEnumerable items, int position)
        object ElementAtOrDefault(this IEnumerable items, int position)
        void ForEach(this IEnumerable enumerable, Action<object> action)
        IEnumerable Where(this IEnumerable source, Func<object,bool> predicate)
        bool Contains(this IEnumerable source, object value)

namespace CodeBrix.Platform.Extensions.Collections

  static class MemoryExtensions — allocation-free projections over
  Span<T>/Memory<T>:
        void SelectToSpan<TIn,TOut>(...)                  (4 overloads)
        Span<TValue> WhereToSpan<TValue>(...)
        Memory<TOut> SelectToMemory<TIn,TOut>(...)        (3 overloads)
        Memory<TValue> WhereToMemory<TValue>(...)         (2 overloads)
        Memory<TResult> WhereToMemory<TValue,TResult>(...)
        int Count<T>(this Span<T> span, Func<T,bool> predicate)
        bool Any<T>(this Span<T> span, Func<T,bool> predicate)
        Dictionary<TKey,TValue> ToDictionary<TIn,TKey,TValue>(
                     this Span<TIn> span, Func<TIn,TKey> keySelector,
                     Func<TIn,TValue> valueSelector)
        double Sum(this Span<double> span)
        double Sum<TIn>(this Span<TIn> span, Func<TIn,double> selector)
        Span<TValue> SliceClamped<TValue>(this Span<TValue> span, int start,
                                          int range)

  class WeakAttachedDictionary<TOwner,TKey> where TOwner : class,
                                            where TKey : class
        TValue GetValue<TValue>(TOwner owner, TKey key,
                                Func<TValue> defaultSelector = null)
        TValue GetValue<TValue>(TOwner owner, TKey key,
                                Func<TKey,TValue> defaultSelector)
        void SetValue<TValue>(TOwner owner, TKey key, TValue value)
        void CopyValues(TOwner existingOwner, TOwner newOwner)
      Attaches arbitrary values to instances you do not own, keyed by owner,
      without keeping the owner alive (ConditionalWeakTable inside). THREAD-
      SAFE.

  class UnsafeWeakAttachedDictionary<TOwner,TKey> — same shape (GetValue with
  Func<TValue>, SetValue, CopyValues), NOT thread-safe; only ever touch it
  from a single thread. Use it when the owner is a UI object confined to the
  UI thread and you want to skip the concurrent-dictionary overhead.


CORE API REFERENCE — GROUPING
=============================

namespace CodeBrix.Platform.Extensions

  static class EnumerableExtensions (grouping part)
        IEnumerable<IEnumerable<T>> GroupBy<T>(this IEnumerable<T> items,
                     int itemsByGroup)                  <- fixed-size chunks
        IEnumerable<IBindableGrouping<string,T>> GroupAlphabetically<T>(
                     this IEnumerable<T> items, Func<T,string> keySelector,
                     bool includeEmptyGroups = true)
        IEnumerable<IBindableGrouping<TKey,TItem>> GroupBy<TKey,TItem>(
                     this IEnumerable<TItem> items,
                     params GroupDescriptor<TKey,TItem>[] descriptors)

  interface IBindableGrouping<TKey,TItem> : IGrouping<TKey,TItem>
        (adds nothing of its own; it exists so the group can be data-bound)

  class BindableGroup<TKey,TItem> : IEnumerable<TItem>,
                                    IBindableGrouping<TKey,TItem>
        BindableGroup(TKey key)
        BindableGroup(TKey key, IList<TItem> items)
        TKey Key { get; }
        IList<TItem> Items { get; }     <- a real IList, so an items control
                                           can bind to it and it can grow
        bool HasItems { get; }
        IEnumerator<TItem> GetEnumerator()
        Equals is overridden to compare the Key AND the item sequence
        against any IGrouping<TKey,TItem>; GetHashCode mixes the key hash
        with the item hashes, so it changes as Items changes — do not use a
        BindableGroup as a dictionary key while you are still filling it.

  class GroupDescriptor<TKey,TItem>
        GroupDescriptor(TKey key)
        GroupDescriptor(TKey key, Func<TItem,bool> selector,
                        bool required = false)
        TKey Key { get; }
        Func<TItem,bool> Selector { get; }
        bool Required { get; }          <- required groups are emitted even
                                           when they end up empty

  class GroupDescriptorComparer<TKey> : IComparer<TKey>
        GroupDescriptorComparer(IEnumerable<TKey> descriptors)
        int Compare(TKey x, TKey y)
      Orders keys by their position in the descriptor list, so groups sort
      in the order you declared rather than alphabetically. Keys that are not
      in the list sort after every known key.


CORE API REFERENCE — EQUALITY AND COMPARISON
============================================

namespace CodeBrix.Platform.Extensions.Equality

  static/class FuncEqualityComparer (non-generic factory)
        static IEqualityComparer<T> Create<T,TValue>(
                     Func<T,TValue> valueSelector)
        static IEqualityComparer<T> Create<T,TValue>(
                     Func<T,TValue> valueSelector,
                     IEqualityComparer<TValue> valueEqualityComparer)

  class FuncEqualityComparer<T> : IEqualityComparer<T>
        FuncEqualityComparer(Func<T?,T?,bool> predicate)
        FuncEqualityComparer(Func<T?,T?,bool> equals, Func<T,int> getHashCode)
        static IEqualityComparer<T> Create<TValue>(
                     Func<T,TValue> valueSelector [, IEqualityComparer<TValue>
                     valueEqualityComparer])
        bool Equals(T? x, T? y)          int GetHashCode(T obj)

  class FuncEqualityComparer<T,TValue> : IEqualityComparer<T>
        FuncEqualityComparer(Func<T,TValue> valueSelector,
                     IEqualityComparer<TValue>? valueEqualityComparer = null)

  class CollectionEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
        CollectionEqualityComparer(IEqualityComparer<T>? comparer = null)
        bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        int GetHashCode(IEnumerable<T> source)

  class WeakReferenceEqualityComparer<T>
                     : IEqualityComparer<System.WeakReference<T>>
        bool Equals(WeakReference<T> w1, T t2)
        bool Equals(WeakReference<T>? w1, WeakReference<T>? w2)
        int GetHashCode(WeakReference<T> w)

  interface IKeyEquatable            int GetKeyHashCode();
                                     bool KeyEquals(object other);
  interface IKeyEquatable<T>         int GetKeyHashCode();
                                     bool KeyEquals(T other);
      "Same identity, possibly different version" — two instances of the same
      logical entity compare KeyEquals-true even when Equals is false.

  class KeyEqualityComparer : IEqualityComparer
        static IEqualityComparer Default { get; }
        KeyEqualityComparer(IEqualityComparer? fallbackComparer = null)
        int GetHashCode(object obj)
  class KeyEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
        static IEqualityComparer<T> Default { get; }
        KeyEqualityComparer(IEqualityComparer<T>? fallbackComparer = null)
        bool Equals(T? left, T? right)   int GetHashCode(T obj)
      Uses IKeyEquatable when the items implement it, otherwise the fallback
      comparer. This is the comparer to pass to ObservableCollection Update
      when items are re-fetched versions of the same entities.

namespace CodeBrix.Platform.Extensions.Comparison

  class FuncComparer<T> : IComparer<T>
        static IComparer<T> Create<TValue>(Func<T?,TValue> valueSelector)
        FuncComparer(Func<T?,T?,int> compare)
        int Compare(T? x, T? y)

namespace CodeBrix.Platform.Extensions.Core.Comparison

  class FastTypeComparer : IEqualityComparer<Type>
        static FastTypeComparer Default { get; }
        bool Equals(Type? x, Type? y)      <- reference equality
        int GetHashCode(Type obj)          <- RuntimeHelpers.GetHashCode
      Skips Type.Equals entirely; use it as the comparer of a
      Dictionary<Type, ...> on a hot path.

namespace CodeBrix.Platform.Extensions.Core.Equality

  static class EqualityComparerExtensions
        IEqualityComparer ToEqualityComparer<T>(
                     this IEqualityComparer<T> genericComparer)
        IEqualityComparer<T> ToEqualityComparer<T>(
                     this IEqualityComparer comparer)


CORE API REFERENCE — STRINGS, PRIMITIVES, MISC
==============================================

namespace CodeBrix.Platform.Extensions

  static class StringExtensions
        bool IsNullOrEmpty(this string instance)
        bool IsNullOrWhiteSpace(this string instance)
        bool HasValue(this string instance)
        bool HasValueTrimmed(this string instance)
        bool Contains(this string instance, string value,
                      StringComparison comparisonType)
        bool IsNumber(this string instance)     bool IsDigit(this string s)
        string JoinBy(this IEnumerable<string> items, string joinBy)
        string InvariantCultureFormat(this string instance,
                                      params object[] array)
        string CurrentCultureFormat(this string instance,
                                    params object[] array)
        string Left(this string instance, int length)
        string Right(this string instance, int length)
        string Append(this string target, string chunk)
        string Append(this string target, string chunk,
                      Func<string,bool> condition)
        string AppendIfMissing(this string target, string chunk)
        string TrimStart(this string source, string trimText
                         [, StringComparison comparisonType])
        string TrimEnd(this string source, string trimText
                       [, StringComparison comparisonType])
        string Indent(this string text, int indentCount = 1)
        string UppercaseFirst(this string s)
        string RemoveDiacritics(this string s)
        static string Format(string format, params object[] args)
        static string Format(IFormatProvider provider, string format,
                             params object[] args)
      The extension methods work on a null instance (IsNullOrEmpty,
      HasValue and friends are null-safe by construction). Note that
      HasValue and HasValueTrimmed are implemented identically — both are
      !string.IsNullOrWhiteSpace(instance) — so "   ".HasValue() is false.

  static class DoubleExtensions
        double Clamp(this double valueToClamp, double minimum, double maximum)
        double RoundAwayFromZero(this double number)
        double EnsureNumber(this double value, double targetValueIfNan = 0)

  static class StreamExtensions
        Task<byte[]> ReadBytesAsync(this Stream stream)
        byte[] ReadBytes(this Stream stream)
        string ReadToEnd(this Stream stream)
        string ReadToEnd(this Stream stream, Encoding encoding)
        bool StartsWith(this Stream stream, byte[] start)
        MemoryStream ToMemoryStream(this Stream source)
        Stream ToSeekable(this Stream stream)

  static class TextWriterExtensions
        void Write(this TextWriter writer, string format,
                   params object[] args)
        void WriteLine(this TextWriter writer, string format,
                       params object[] args)
        void Write(this TextWriter writer, IFormatProvider formatProvider,
                   string format, params object[] args)
        void WriteLine(this TextWriter writer, IFormatProvider formatProvider,
                       string format, params object[] args)

  static class UriExtensions
        static string EscapeDataString(string value)
      NOT an extension method despite the class name — call
      UriExtensions.EscapeDataString(s). It chunks the input at 10,000
      characters so it works on strings that are too long for
      Uri.EscapeDataString.

  static class MatchExtensions
        IEnumerable<Match> AsEnumerable(this Match match)
      Walks Match.NextMatch() lazily, turning a first match into the whole
      match sequence.

  static class WeakReferenceExtensions   (on System.WeakReference<T>)
        bool HasTarget<T>(this WeakReference<T> wr) where T : class
        T GetTarget<T>(this WeakReference<T> wr) where T : class
        T FindOrCreate<T>(this WeakReference<T> wr, Func<T> factory)
                                                    where T : class

  static class EnumHelper
        static string[] GetNames<T>()
        static T[] GetValues<T>()
      Faster than Enum.GetNames/GetValues because the results are NOT sorted;
      GetNames and GetValues return matching orders. Memoize if hot.

  enum DateTimeUnit  [Flags]
        Year = 1, Month = 2, Day = 4, Hour = 8, Minute = 16, Second = 32,
        Millisecond = 64,
        ToMonth = Year|Month, ToDay = ToMonth|Day, ToHour = ToDay|Hour,
        ToMinute = ToHour|Minute, ToSecond = ToMinute|Second,
        ToMillisecond = ToSecond|Millisecond
      A precision/truncation vocabulary you can accept in your own APIs; this
      package ships no DateTime methods that consume it.

  class CachedTuple / CachedTuple<T1,T2> / <T1,T2,T3> / <T1,T2,T3,T4>
        static CachedTuple<T1,T2> Create<T1,T2>(T1 item1, T2 item2)
        static CachedTuple<T1,T2,T3> Create<T1,T2,T3>(...)
        static CachedTuple<T1,T2,T3,T4> Create<T1,T2,T3,T4>(...)
        T1 Item1 { get; } ... T4 Item4 { get; }
        static readonly IEqualityComparer<CachedTuple<...>> Comparer
      A tuple whose hash code is computed ONCE in the constructor. Use it,
      with its .Comparer, as a composite dictionary key on a hot path. This
      is what the multi-parameter memoizers use internally.

  sealed class LegacyAttribute : Attribute
        LegacyAttribute(string ruleId)
        string RuleId { get; }
      Marks an interface, class, constructor, method, property or event as
      legacy so a static-analysis rule can fail the build. Inherited = false,
      AllowMultiple = true. Pair it with [Obsolete] when you also want a
      compiler warning; this attribute alone produces no diagnostic.


COMPLETE EXAMPLES
=================

1) Async lock around a shared resource (re-entrant on the same async flow)

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeBrix.Platform.Extensions.Threading;

    public sealed class TokenCache
    {
        private readonly FastAsyncLock _gate = new FastAsyncLock();
        private string _token;

        public async Task<string> GetTokenAsync(CancellationToken ct)
        {
            using (await _gate.LockAsync(ct))
            {
                if (_token == null)
                {
                    _token = await FetchAsync(ct);
                    // Re-entering the same lock on this async flow is safe:
                    await TouchAsync(ct);
                }

                return _token;
            }
        }

        private async Task TouchAsync(CancellationToken ct)
        {
            using (await _gate.LockAsync(ct))   // re-entrant, no deadlock
            {
                await Task.Yield();
            }
        }

        private Task<string> FetchAsync(CancellationToken ct)
            => Task.FromResult(Guid.NewGuid().ToString("N"));
    }

2) Composing disposables for a subscription lifetime

    using System;
    using System.Collections.Generic;
    using System.IO;
    using CodeBrix.Platform.Extensions;             // DisposableAdd
    using CodeBrix.Platform.Extensions.Disposables; // the rest

    public sealed class Watcher : IDisposable
    {
        private readonly CompositeDisposable _subscriptions
            = new CompositeDisposable();
        private readonly SerialDisposable _current = new SerialDisposable();

        public Watcher(FileSystemWatcher watcher)
        {
            // 1. an arbitrary cleanup action
            _subscriptions.Add(() => Console.WriteLine("watcher torn down"));

            // 2. an explicit disposable, registered fluently
            watcher.DisposeWith(_subscriptions);

            // 3. a slot that always holds at most one live subscription:
            //    assigning a new value disposes the previous one
            _current.DisposeWith(_subscriptions);
            _current.Disposable = Disposable.Create(
                () => Console.WriteLine("first pass done"));
            _current.Disposable = Disposable.Create(
                () => Console.WriteLine("second pass done")); // 1st disposed

            // 4. temporary membership: dispose the handle to remove the item
            var registry = new List<string>();
            IDisposable membership = registry.DisposableAdd("watcher");
            _subscriptions.Add(membership);
        }

        public void Dispose() => _subscriptions.Dispose();
    }

    // After Dispose(), the composite is inert but still safe to use:
    // anything you Add() is disposed immediately instead of being kept.

3) Lock-free state update with Transactional

    using System;
    using System.Collections.Immutable;
    using CodeBrix.Platform.Extensions;

    public sealed class Registry
    {
        // Both fields are updated without any lock.
        private ImmutableDictionary<string, int> _counts
            = ImmutableDictionary<string, int>.Empty;
        private State _state = new State(0, "idle");

        public int Touch(string key)
            // UpdateItem's factory receives (key, currentValue); a missing
            // key yields default(TValue), i.e. 0 here.
            => Transactional.UpdateItem(ref _counts, key,
                   (k, current) => current + 1);

        public int GetOrAdd(string key)
            => Transactional.GetOrAdd(ref _counts, key, k => k.Length);

        public void Advance(string name)
            // The selector MUST be pure: on a lost race it runs again.
            => Transactional.Update(ref _state,
                   s => new State(s.Version + 1, name));

        public sealed class State
        {
            public State(int version, string name)
            { Version = version; Name = name; }
            public int Version { get; }
            public string Name { get; }
        }
    }

4) Memoization and retry

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeBrix.Platform.Extensions;

    // Compute once, then serve from cache. AsMemoized is NOT thread-safe.
    Func<string, int> lengthOf = s => { Console.WriteLine("computing");
                                        return s.Length; };
    var cachedLength = lengthOf.AsMemoized();
    cachedLength("hello");      // prints "computing", returns 5
    cachedLength("hello");      // no print, returns 5 from the cache

    // Same idea, safe to call from several threads at once.
    Func<int> expensive = () => Environment.TickCount;
    var onceEver = expensive.AsLockedMemoized();

    // 4 total attempts, 250 ms apart; the 4th failure propagates.
    // (httpClient, url and cancellationToken come from the caller.)
    Func<CancellationToken, Task<string>> fetch =
        ct => httpClient.GetStringAsync(url, ct);

    string body = await fetch.Retry(
        cancellationToken, tries: 4,
        retryDelay: TimeSpan.FromMilliseconds(250));

    // Memoize per-instance without keeping the instance alive:
    int idLength = someEntity.ApplyMemoized(e => e.Id.Length);

5) Bootstrapping logging, then logging

    using System;
    using Microsoft.Extensions.Logging;
    using CodeBrix.Platform.Extensions;          // LogExtensionPoint
    using CodeBrix.Platform.Extensions.Logging;  // LogExtensions

    public static class Program
    {
        public static void Main()
        {
            // Do this ONCE, before anything calls .Log().
            LogExtensionPoint.AmbientLoggerFactory =
                LoggerFactory.Create(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Debug);
                    builder.AddConsole();   // provider package required
                });

            new Service().Run();
        }
    }

    public sealed class Service
    {
        public void Run()
        {
            // this.Log() -> ILogger named "Service"
            this.Log().Info("service starting");
            this.Log().DebugFormat("worker count: {0}", 4);

            // Only builds the message when Debug is enabled:
            this.Log().DebugIfEnabled(() => $"state = {Describe()}");

            try { DoWork(); }
            catch (Exception ex) { this.Log().Error("work failed", ex); }

            // Logger for a type you do not have an instance of:
            typeof(Service).Log().Warn("static-side warning");
        }

        private void DoWork() { /* ... */ }
        private string Describe() => "ready";
    }

    // Alternative bootstrap: register an ILoggerFactory with
    // CodeBrix.ServiceLocation instead of assigning AmbientLoggerFactory.
    // LogExtensionPoint resolves ILoggerFactory from
    // ServiceLocator.Current on first use when a provider is set.

6) Differential update of a bound collection, plus grouping

    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using CodeBrix.Platform.Extensions;
    using CodeBrix.Platform.Extensions.Equality;

    public sealed class Person : IKeyEquatable<Person>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int GetKeyHashCode() => Id;
        public bool KeyEquals(Person other) => other?.Id == Id;
    }

    var bound = new ObservableCollection<Person>(existingPeople);

    // Patch in place: existing instances that match are KEPT, so selection
    // and bindings survive. KeyEqualityComparer matches by entity key.
    var results = bound.UpdateWithResults(
        freshlyFetchedPeople,
        tryDispose: false,
        comparer: KeyEqualityComparer<Person>.Default);

    Console.WriteLine($"+{results.Added.Count()} " +
                      $"~{results.Moved.Count()} " +
                      $"-{results.Removed.Count()}");

    // Group into declared buckets, keeping the declared order.
    var groups = bound.GroupBy(
        new GroupDescriptor<string, Person>("A-M",
            p => p.Name[0] <= 'M', required: true),
        new GroupDescriptor<string, Person>("N-Z",
            p => p.Name[0] > 'M', required: true));

    foreach (IBindableGrouping<string, Person> g in groups)
    {
        Console.WriteLine($"{g.Key}: {g.Count()}");
    }

    // Or bucket alphabetically by first letter:
    var alpha = bound.GroupAlphabetically(p => p.Name,
                                          includeEmptyGroups: false);


MINIMUM VIABLE PROJECT
======================

MyApp.csproj

    <Project Sdk="Microsoft.NET.Sdk">
      <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>disable</Nullable>
      </PropertyGroup>
      <ItemGroup>
        <PackageReference
          Include="CodeBrix.Platform.Extensions.ApacheLicenseForever"
          Version="*" />
      </ItemGroup>
    </Project>

Program.cs

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CodeBrix.Platform.Extensions;
    using CodeBrix.Platform.Extensions.Disposables;
    using CodeBrix.Platform.Extensions.Threading;

    internal static class Program
    {
        private static readonly FastAsyncLock Gate = new FastAsyncLock();

        private static async Task Main()
        {
            using var cleanup = new CompositeDisposable();
            cleanup.Add(() => Console.WriteLine("done"));

            var names = new List<string> { "  ", "ada", null, "grace" };

            names.Safe()
                 .Trim()                        // drop the nulls
                 .Where(n => n.HasValueTrimmed())
                 .ForEach(n => Console.WriteLine(n.UppercaseFirst()));

            using (await Gate.LockAsync(CancellationToken.None))
            {
                Console.WriteLine("inside the async lock");
            }
        }
    }

Replace Version="*" with the version your project pins; this file shows the
reference, not a version policy.


PERFORMANCE TIPS
================

  * Pick the right memoizer. AsMemoized uses a bare Dictionary with no lock
    and is the fastest, but it is only safe when calls are serialized.
    AsLockedMemoized adds a double-checked lock (parameterless) or a
    concurrent dictionary (keyed). Never share an AsMemoized delegate across
    threads — a concurrent write can corrupt the Dictionary.
  * Memoizers never evict. The keyed overloads hold every key and result for
    the life of the delegate. Use them for bounded key spaces (types, enum
    values, configuration keys), never for user input or request ids.
  * For per-instance caching that must not leak, use ApplyMemoized /
    AsWeakMemoized: the cache holds the source weakly, so the instance can
    still be collected.
  * FastTaskCompletionSource is its own awaiter and allocates less than
    TaskCompletionSource<T>; `await source;` avoids materializing .Task at
    all. Only touch .Task when you must hand a Task to someone else.
  * Transactional.Update is lock-free (Interlocked.CompareExchange) and beats
    a lock under low contention. Under HIGH contention it spins and re-runs
    the selector, so an expensive selector plus heavy contention is worse
    than a lock. Keep the selector cheap and side-effect free.
  * FastAsyncLock is re-entrancy-aware, which costs one AsyncLocal read on
    the fast path. When you do not need re-entrancy and never await inside
    the critical section, a plain lock is cheaper.
  * SelectToArray / SelectToList / WhereToList size the destination from the
    source count and skip the LINQ iterator machinery; prefer them in hot
    loops over .Select().ToArray().
  * MemoryExtensions' SelectToSpan / WhereToSpan write into a caller-supplied
    Span, so a projection over a stack-allocated buffer allocates nothing.
  * Use CachedTuple (with its static .Comparer) as a composite dictionary
    key: the hash is computed once at construction instead of on every
    lookup.
  * FastTypeComparer.Default makes Dictionary<Type,...> lookups reference-
    equality fast; EnumHelper.GetNames/GetValues skip the BCL's sorting.
  * Prefer the *IfEnabled logging helpers (DebugIfEnabled and friends) when
    the message needs interpolation or ToString() work — the closure only
    runs if that level is enabled.


COMMON PITFALLS TO AVOID
========================

  * Namespaces do not follow folders. NullDisposable lives in
    CodeBrix.Platform.Extensions, NOT ...Extensions.Disposables. The
    collection extension classes (EnumerableExtensions, CollectionExtensions,
    ListExtensions, DictionaryExtensions, QueueExtensions, StackExtensions,
    ObservableCollectionExtensions) live in the root namespace, NOT
    ...Extensions.Collections — only MemoryExtensions and the two
    WeakAttachedDictionary types are in .Collections. Transactional and
    LogExtensionPoint are in the root namespace too.
  * The ForEach overload that takes Action<KeyValuePair<int,T>> delegates to
    ForEach(Action<int,T>) and receives (index, item) pairs; all three
    ForEach overloads are eager and return the source.
  * ForEach is EAGER and returns the source; Do is LAZY and does nothing
    until enumerated. Reaching for Do and forgetting to enumerate is a
    silent no-op.
  * MinBy / MaxBy here return a tuple `(TSource Item, TComparable Value)`,
    not the item. On .NET 10 System.Linq also has MinBy/MaxBy with the same
    parameter shape, so with both namespaces imported the call can be
    ambiguous — write EnumerableExtensions.MinBy(source, selector)
    explicitly, and remember to read .Item.
  * MaxOrDefault<TSource,TResult>(source, selector, defaultValue) is NOT an
    extension method (no `this`); call it as
    EnumerableExtensions.MaxOrDefault(...). Likewise
    UriExtensions.EscapeDataString is a plain static.
  * MinBy/MaxBy throw InvalidOperationException on an empty sequence.
  * Adding to a CompositeDisposable that is already disposed disposes the new
    item immediately and silently — no exception, no membership. Check
    IsDisposed if that matters. Remove(item) also disposes the item.
  * Assigning SerialDisposable.Disposable disposes whatever was there before,
    and assigning after the SerialDisposable itself is disposed disposes the
    incoming value at once.
  * Disposable.Create(null) throws ArgumentNullException. The action it wraps
    runs at most once no matter how often Dispose() is called.
  * Logging silently goes nowhere until AmbientLoggerFactory is set (or a
    CodeBrix.ServiceLocation provider can resolve ILoggerFactory). No exception
    is thrown — if you see no output, you skipped the bootstrap. A provider
    set on the OLD CommonServiceLocator package is not observed; see the
    breaking-change note in the logging-bootstrap section.
  * The logger returned by instance.Log() is cached per static type T on
    first use. Set the factory before the first .Log() call, not after.
  * instance.Log() names the logger after the STATIC type of the expression.
    Calling it through an `object`-typed variable yields a System.Object
    logger. Use this.Log() inside the class or typeof(X).Log().
  * The LogExtensions level names are Info and Warn, not Information and
    Warning; the Format variants take a composite format string while the
    non-Format ones take a plain message plus an optional Exception.
  * Transactional.Update's selector can run several times when threads race.
    Never put a side effect (I/O, counters, event raising) inside it.
  * Transactional's collection helpers require the System.Collections.
    Immutable interfaces (IImmutableList<T>, IImmutableDictionary<TKey,
    TValue>, IImmutableQueue<T>). They do not work on List<T> or
    Dictionary<TKey,TValue>.
  * ObservableCollectionExtensions.Update with tryDispose: true disposes
    removed items AND incoming items that were not added. Only pass true
    when the incoming items are DIFFERENT instances matched by Equals;
    passing true with shared instances disposes objects that are still live.
  * UnsafeWeakAttachedDictionary is not thread-safe by design — use
    WeakAttachedDictionary unless you can guarantee single-thread access.
  * AsyncEvent has no parameterless constructor: new AsyncEvent(0).
    AsyncEvent.Wait returns false on an already-cancelled token instead of
    throwing OperationCanceledException.
  * FastAsyncLock re-entrancy is per ExecutionContext. Work you push onto
    another thread with Task.Run does NOT inherit the lock and will block.
  * Retry's `tries` is the total attempt count, not the retry count:
    tries: 1 means a single attempt with no retry, and tries: 0 loops
    forever because the counter never reaches zero.
  * The keyed memoizers cache by key forever and treat a null key specially
    (a separate single-value cache). They cache successful results only —
    an exception is not cached and the call will be retried.
  * Null is a placeholder type with a private constructor. You can never
    construct one; pass null wherever a Null is expected.
  * Do not write using directives for the upstream namespaces of the project
    this was vendored from. They are not present in this assembly.


WHAT THIS PACKAGE DOES NOT DO
=============================

  * It does not provide the upstream "Compatibility" helper package. The
    upstream project also shipped a large compatibility library (async
    helpers, builders, conversion, decorators, events, expressions,
    reflection, serialization, validation). None of it is vendored here. If
    you need one of those types you must source it elsewhere; it will not
    appear under CodeBrix.Platform.Extensions.
  * It is not a drop-in replacement for the upstream packages. The
    namespaces are renamed, so upstream code does not compile unchanged.
  * It is not a logging framework. It has no ILogger providers, sinks or
    configuration; you bring Microsoft.Extensions.Logging providers.
  * It is not a dependency-injection container. CodeBrix.ServiceLocation is
    used only as an optional lookup for ILoggerFactory.
  * It ships no UI types, no XAML, no controls and no platform abstractions.
  * It ships no MSBuild props/targets, no analyzers, no source generators and
    no native assets — the netstandard2.0 target exists so that generator and
    analyzer projects can CONSUME these helpers, not because the package
    contains a generator.
  * It has no DateTime extension methods. DateTimeUnit is a vocabulary enum
    only.
  * The vendored types have no XML documentation in the package (doc-file
    generation is off for the vendored source), so IntelliSense shows
    signatures without summaries. Use this file as the reference.


WORKING EXAMPLES ON GITHUB
==========================

Runnable, compiled usage of the public API lives in the test project:

  https://github.com/ellisnet/CodeBrix.Platform.Extensions/tree/main/tests/CodeBrix.Platform.Extensions.Tests

  HelperBehaviorTests.cs
      CompositeDisposable with two Disposable.Create children;
      Disposable.Create running its action exactly once across two
      Dispose() calls; NullDisposable.Instance as a singleton;
      FuncEqualityComparer.Create<string,int> comparing by projection;
      EnumerableExtensions.None and .ForEach; StringExtensions.HasValue and
      .IsNullOrWhiteSpace; FastAsyncLock.LockAsync acquired, released and
      re-acquired.
  NamespaceMappingTests.cs
      The authoritative list of where the renamed types live — including
      the root-namespace placement of NullDisposable and the confirmation
      that no type answers to an upstream namespace any more.
  AssemblyMetadataTests.cs
      Assembly simple name, target framework, and the guarantee that every
      exported type sits under the CodeBrix.Platform.Extensions root.

Repository root: https://github.com/ellisnet/CodeBrix.Platform.Extensions


QUICK REFERENCE CARD
====================

  Install       dotnet add package
                CodeBrix.Platform.Extensions.ApacheLicenseForever
  Targets       net10.0 and netstandard2.0 (the latter for Roslyn source
                generators / analyzers)
  License       Apache-2.0
  Deps          CodeBrix.ServiceLocator.MsplLicenseForever,
                Microsoft.Extensions.Logging
                (+ System.Memory, System.Collections.Immutable,
                System.Threading.Tasks.Extensions on netstandard2.0)

  Async lock            using (await gate.LockAsync(ct)) { ... }
  Ad-hoc disposable     Disposable.Create(() => ...)   / Disposable.Empty
  Bag of disposables    new CompositeDisposable(); c.Add(() => ...)
  Swap-one slot         serial.Disposable = next;   // disposes previous
  Attach to a bag       thing.DisposeWith(composite)
  Lock-free field       Transactional.Update(ref _f, x => Next(x))
  Immutable dict add    Transactional.GetOrAdd(ref _d, key, k => Make(k))
  Cache a func          f.AsMemoized()   /   f.AsLockedMemoized()
  Per-instance cache    obj.ApplyMemoized(o => Compute(o))
  Retry                 f.Retry(ct, tries: 3, retryDelay: delay)
  Curry                 f.CurryFirst(arg)  /  f.CurryLast(arg)
  Logging bootstrap     LogExtensionPoint.AmbientLoggerFactory = factory;
  Log                   this.Log().Info("...") / typeof(X).Log().Error(m, ex)
  Cheap-message log     log.DebugIfEnabled(() => $"...")
  Patch a bound list    list.UpdateWithResults(fresh, comparer: cmp)
  Group in order        items.GroupBy(new GroupDescriptor<TK,TI>(...), ...)
  Chunk                 items.GroupBy(itemsByGroup: 50)
  Comparer from lambda  FuncEqualityComparer.Create<T,TValue>(x => x.Key)
  Entity-key comparer   KeyEqualityComparer<T>.Default   (T : IKeyEquatable)
  Fast Type key         FastTypeComparer.Default
  Weak side table       new WeakAttachedDictionary<TOwner,TKey>()
  Null-safe sequence    src.Safe().Trim()
  Null-safe string      s.HasValue() / s.HasValueTrimmed()

  Namespace traps       NullDisposable, EnumerableExtensions,
                        CollectionExtensions, ListExtensions,
                        DictionaryExtensions, QueueExtensions,
                        StackExtensions, ObservableCollectionExtensions,
                        Transactional, LogExtensionPoint
                            -> CodeBrix.Platform.Extensions (root)
                        MemoryExtensions, WeakAttachedDictionary,
                        UnsafeWeakAttachedDictionary
                            -> CodeBrix.Platform.Extensions.Collections
                        FuncComparer -> ...Extensions.Comparison
                        FastTypeComparer -> ...Extensions.Core.Comparison
                        EqualityComparerExtensions
                            -> ...Extensions.Core.Equality
