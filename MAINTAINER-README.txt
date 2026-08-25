================================================================================
MAINTAINER-README: CodeBrix.Platform.Extensions
Notes for people and agents MAINTAINING this repository — not for package
consumers
================================================================================

Package consumers should read AGENT-README.txt instead. This file covers
building, testing, packaging, provenance and the repo-specific conventions.


PURPOSE AND SCOPE
=================

This repository produces exactly one NuGet package:

  PackageId    CodeBrix.Platform.Extensions.ApacheLicenseForever
  Project      src/CodeBrix.Platform.Extensions/
                   CodeBrix.Platform.Extensions.csproj
  AssemblyName CodeBrix.Platform.Extensions
  RootNamespace CodeBrix.Platform.Extensions
  Consumer doc AGENT-README.txt (repo root) — also packed into the nupkg

The library exists so that CodeBrix.Platform can depend on ONE CodeBrix-owned
package instead of a fan-out of third-party helper package references. Seven
upstream helper projects are merged into a single assembly, one sub-folder per
upstream project so provenance stays traceable.


REPOSITORY LAYOUT
=================

  CodeBrix.Platform.Extensions.slnx        solution (Solution Items + Tests
                                           folder + the library project)
  src/CodeBrix.Platform.Extensions/
      CodeBrix.Platform.Extensions.csproj
      InternalsVisibleTo.cs                -> CodeBrix.Platform.Extensions.Tests
      Extensions/        (26 files) <- upstream general-extensions project
      Collections/       (14 files) <- upstream collections project
      Disposables/       (13 files) <- upstream disposables project
      Equality/          ( 8 files) <- upstream equality project
      Logging/           ( 2 files) <- upstream logging project
      LoggingSingleton/  ( 1 file ) <- upstream logging-singleton project
      Threading/         ( 6 files) <- upstream threading project
  tests/CodeBrix.Platform.Extensions.Tests/
      AssemblyMetadataTests.cs
      NamespaceMappingTests.cs
      HelperBehaviorTests.cs
  AGENT-README.txt      MAINTAINER-README.txt   EXTRAS-README.txt
  README-INDEX.txt      README.md               LICENSE (Apache-2.0)
  THIRD-PARTY-NOTICES.txt                       icon-codebrix-128.png

IMPORTANT: the folder names do NOT match the namespaces. The folders record
which upstream project a file came from; the namespaces follow the upstream
namespace mapping. Files under Collections/ mostly declare the ROOT namespace,
Threading/Transactional*.cs declares the ROOT namespace, and
LoggingSingleton/LogExtensionPoint.cs declares the ROOT namespace. Do not
"tidy" a namespace to match its folder — that is a breaking API change and it
would break tests/NamespaceMappingTests.cs.


BUILDING
========

  dotnet restore CodeBrix.Platform.Extensions.slnx
  dotnet build   CodeBrix.Platform.Extensions.slnx -c Release

The library multi-targets netstandard2.0 and net10.0; the test project is
net10.0 only.

WHY netstandard2.0 EXISTS HERE (durable rule)
---------------------------------------------
The CodeBrix family is .NET 10 only. The single standing exception is code
that has to be loaded by the Roslyn compiler — source generators and
analyzers must be built against netstandard2.0. This library multi-targets
netstandard2.0 so that a generator or analyzer project can consume these
helpers. That is the ONLY reason the extra target framework is there; it is
not general downlevel support, and no other CodeBrix rule is relaxed by it.
Do not remove the netstandard2.0 target, and do not add further target
frameworks.

Per-target package references (net10 has these in-box, netstandard2.0 needs
explicit polyfills):

  all targets       CommonServiceLocator
  net10.0           Microsoft.Extensions.Logging
  netstandard2.0    Microsoft.Extensions.Logging
                    System.Collections.Immutable
                    System.Memory
                    System.Threading.Tasks.Extensions

LangVersion is pinned to `latest` for all targets: netstandard2.0 would
otherwise default to C# 7.3 and the vendored source uses newer syntax.

DefineConstants adds IS_THREADING_PROJECT. Threading/Transactional.cs is
shared upstream through that symbol and is `internal` without it; this bundle
includes the threading sources, so the symbol is defined and Transactional is
public. Removing the symbol silently makes Transactional internal.

SITUATIONAL PORT EXCEPTIONS (vendored source only)
--------------------------------------------------
These two family rules are relaxed here, exactly as they are in the other
vendored-source CodeBrix libraries. They apply ONLY to the vendored upstream
files, never to new CodeBrix code:

  * <GenerateDocumentationFile>false</GenerateDocumentationFile>
    The upstream sources have hundreds of undocumented public members
    (CS1591) plus some malformed upstream doc tags. Retrofitting 470+ doc
    comments onto faithful third-party source is out of scope.
  * <NoWarn>$(NoWarn);CS8632;CA2022</NoWarn>
    CS8632 = stray nullable-reference annotations under a Nullable-disabled
    context; CA2022 = an upstream stream-read pattern. Both originate in the
    upstream source. Nullable reference types stay OFF (no <Nullable>enable).

Upstream `#if` framework-selection directives are preserved verbatim rather
than stripped, and upstream block-scoped namespaces are kept, so a future
re-sync against upstream stays a readable diff. Only one public member is
excluded by the preprocessor on the shipped targets: a Zip overload in
Collections/EnumerableExtensions.cs sits inside a WINDOWS_PHONE block that
also contains an #error, so it is dead code — do not document it as API.


TESTING
=======

  dotnet test CodeBrix.Platform.Extensions.slnx

No opt-in environment variables, no special prep, no external services. The
test project uses xUnit v3 + SilverAssertions + coverlet.collector and gets
internals access through src/CodeBrix.Platform.Extensions/InternalsVisibleTo.cs.

The suite is a structural / smoke suite, not a 1:1 port of the upstream unit
tests. It asserts:
  * the assembly's simple name and that it targets net10 (AssemblyMetadataTests)
  * that every exported type sits under the CodeBrix.Platform.Extensions root
    and none under an upstream namespace (AssemblyMetadataTests,
    NamespaceMappingTests)
  * that representative helpers from each area behave correctly
    (HelperBehaviorTests)

Tests follow the family conventions: file named <Class>Tests.cs, snake_case
method names, //Arrange //Act //Assert comment markers, and
TestContext.Current.CancellationToken for any API that wants a token
(xUnit1051).

Upstream defect FIXED in this port (2026-08-24): the
EnumerableExtensions.ForEach overload that takes Action<KeyValuePair<int,T>>
called itself and recursed until the stack overflowed. It now delegates to
ForEach(Action<int,T>); the original line is kept as a `//was previously:`
marker and HelperBehaviorTests covers the overload.


PACKAGING AND PUBLISHING
========================

GeneratePackageOnBuild is true, so every build of the library project drops a
.nupkg in its bin/<config> folder.

Versioning is the family date-stamped scheme, computed in the csproj from
System.DateTime.UtcNow: 1.<years since the base year>.<day of year>.<minute
of day UTC>. It is strictly increasing over time and is NOT SemVer — major is
pinned and minor encodes the year, so neither signals API compatibility. Two
builds inside the same UTC minute produce the same version, so never publish
two packages from within one minute. Re-baseline by changing _VersionBaseYear
in the csproj.

What ships in the nupkg, beyond lib/netstandard2.0 and lib/net10.0:

  icon-codebrix-128.png    PackageIcon
  README.md                PackageReadmeFile
  AGENT-README.txt         consumer documentation (packed at the package root)
  THIRD-PARTY-NOTICES.txt  provenance and attribution

MAINTAINER-README.txt, EXTRAS-README.txt and README-INDEX.txt are repo-only
and are NOT packed.

PackageLicenseExpression is Apache-2.0 and PackageRequireLicenseAcceptance is
true. Copyright is fixed at:

  Copyright (c) 2026 Jeremy Ellis and contributors. Portions Copyright (c)
  nventive inc., licensed under Apache-2.0.


PROVENANCE AND VENDORED SOURCES
===============================

Every .cs file under src/ is vendored third-party source at the upstream
project's version 4.1.1, commit 6da975c67063a670626fe7ae19250ff585ba2224 (the
commit the upstream 4.1.1 packages were built from, per the <repository
commit="..."> metadata in each upstream nuspec). THIRD-PARTY-NOTICES.txt holds
the full per-project provenance and the complete modification list; it is the
authoritative record and it ships in the package.

Namespace mapping applied during the port: the upstream root namespace and
its `.Extensions` child BOTH collapse to CodeBrix.Platform.Extensions; every
other upstream `X` becomes CodeBrix.Platform.Extensions.X. The resulting
namespaces are:

  CodeBrix.Platform.Extensions                 (root + .Extensions upstream)
  CodeBrix.Platform.Extensions.Specialized
  CodeBrix.Platform.Extensions.Collections
  CodeBrix.Platform.Extensions.Disposables
  CodeBrix.Platform.Extensions.Equality
  CodeBrix.Platform.Extensions.Comparison
  CodeBrix.Platform.Extensions.Core.Equality
  CodeBrix.Platform.Extensions.Core.Comparison
  CodeBrix.Platform.Extensions.Logging
  CodeBrix.Platform.Extensions.Threading

Every renamed `namespace` line carries a `// was previously: <original>`
provenance comment. Keep those comments; they are how a re-sync against
upstream is done.

Other modifications made during the port, all recorded in
THIRD-PARTY-NOTICES.txt: the target framework set was narrowed (upstream
multi-targeted a much longer list); assembly name and package id were
changed; a corrupted copyright glyph (bytes EF BF BD, an already-mojibake'd
"(c)") in every upstream header was normalized to ASCII "(c)"; doc-file
generation was disabled and the two upstream-origin warnings suppressed. No
managed behavior was changed.

The upstream "Compatibility" helper package was deliberately NOT vendored:
CodeBrix.Platform imports none of its exclusive namespaces. If a future need
appears for a type that lived only there, vendor that specific type in as a
follow-up and extend THIRD-PARTY-NOTICES.txt accordingly.

Note for a future refresh: THIRD-PARTY-NOTICES.txt names a CommonServiceLocator
version that no longer matches the csproj's package reference, and describes
the port as .NET 10 only, which predates the netstandard2.0 target. Correct
both the next time that file is legitimately touched — it is not editable as
part of documentation-only work.


CODING CONVENTIONS
==================

Family rules that apply here:
  * .NET 10 is the target for all CodeBrix code; netstandard2.0 appears here
    only for the Roslyn-host reason above.
  * Nullable reference types are OFF; do not add `?` annotations to reference
    types in new code in this repo.
  * No global usings.
  * New (non-vendored) code IS subject to the family XML-doc rule; the
    doc-file exception above covers the vendored files only.
  * Every packaging library ships an InternalsVisibleTo.cs granting access to
    its `.Tests` assembly.
  * Never rename or renumber diagnostic ids.
  * The word for the upstream project in any documentation written here is
    "the upstream project" / "upstream".

Repo-specific rules:
  * Do not reformat, re-namespace or "modernize" vendored files. Fidelity to
    upstream beats style. If a change is unavoidable, add a
    `//was previously:` comment and record it in THIRD-PARTY-NOTICES.txt.
  * Do not delete upstream `#if` branches even when they are dead on the
    shipped targets.
  * Any new public type must land in one of the ten namespaces listed above;
    adding an eleventh namespace changes the shape of the public surface and
    needs a deliberate decision.


NOTES
=====

  * A downstream fork of the platform embeds some of the same upstream helper
    source directly. When wiring this package into that fork, watch for
    duplicate-type conflicts between the embedded copy and this package.
  * src/ and tests/ may contain left-over local build output (obj/, bin/,
    including old .nupkg and .nuspec files from earlier builds). Those are
    ignored by .gitignore, are not tracked, and are not part of the package —
    never read a version number out of them.
  * CommonServiceLocator is a hard dependency of the whole assembly but is
    used by exactly one type, LoggingSingleton/LogExtensionPoint.cs. If that
    ever becomes a problem, the fix is to make the lookup reflective, not to
    drop the type.
