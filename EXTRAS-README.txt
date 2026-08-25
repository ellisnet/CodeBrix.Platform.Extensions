================================================================================
EXTRAS-README: CodeBrix.Platform.Extensions
Samples, tools and other content in this repository that is not part of a
NuGet package
================================================================================

This repository contains no samples, no demo applications, no tools, no
scripts and no optional test-data sets.

Everything under src/ is packaged. The only non-package content is the test
project.


TEST PROJECT
============

  tests/CodeBrix.Platform.Extensions.Tests/

The single non-package project in the repository. It is a net10.0 xUnit v3
test project (SilverAssertions + coverlet.collector) that references the
library by project reference and is granted internals access through
src/CodeBrix.Platform.Extensions/InternalsVisibleTo.cs. It is not packable and
is not published.

Run it with:

    dotnet test CodeBrix.Platform.Extensions.slnx

Three files:

  AssemblyMetadataTests.cs   assembly simple name, target framework, and the
                             guarantee that every exported type lives under
                             the CodeBrix.Platform.Extensions root namespace
  NamespaceMappingTests.cs   the renamed types resolve at their new full
                             names, and no type answers to an upstream name
  HelperBehaviorTests.cs     behavior smoke tests for one or two
                             representative helpers from each vendored area

Because these tests are the only compiled, runnable usage of the public API
in the repository, they double as the worked examples referenced from
AGENT-README.txt under "WORKING EXAMPLES ON GITHUB".


ROOT FILES THAT ARE NOT DOCUMENTATION
=====================================

  icon-codebrix-128.png   the package icon; packed into the nupkg
  LICENSE                 Apache-2.0 licence text
  THIRD-PARTY-NOTICES.txt provenance and attribution for the vendored source;
                          packed into the nupkg

For the map of the README files themselves, see README-INDEX.txt.
