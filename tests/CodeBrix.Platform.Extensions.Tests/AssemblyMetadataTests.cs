using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using CodeBrix.Platform.Extensions.Disposables;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Platform.Extensions.Tests;

public class AssemblyMetadataTests
{
    private const string ExpectedAssemblyName = "CodeBrix.Platform.Extensions";

    private static Assembly LibraryAssembly => typeof(Disposable).Assembly;

    [Fact]
    public void Library_assembly_simple_name_matches()
        => LibraryAssembly.GetName().Name.Should().Be(ExpectedAssemblyName);

    [Fact]
    public void Library_assembly_targets_net10()
    {
        //Arrange
        var attr = LibraryAssembly.GetCustomAttribute<TargetFrameworkAttribute>();

        //Assert
        attr.Should().NotBeNull();
        attr.FrameworkName.Should().StartWith(".NETCoreApp,Version=v10.");
    }

    [Fact]
    public void Library_exposes_public_types()
        => LibraryAssembly.GetExportedTypes().Length.Should().BeGreaterThan(50);

    [Fact]
    public void No_public_type_remains_in_an_Uno_namespace()
    {
        //Arrange
        var unoTypes = LibraryAssembly.GetExportedTypes()
            .Where(t => t.Namespace is not null && (t.Namespace == "Uno" || t.Namespace.StartsWith("Uno.")))
            .ToList();

        //Assert
        unoTypes.Should().BeEmpty();
    }

    [Fact]
    public void Every_public_type_is_under_the_codebrix_root_namespace()
    {
        //Arrange
        var stray = LibraryAssembly.GetExportedTypes()
            .Where(t => t.Namespace is null || !t.Namespace.StartsWith("CodeBrix.Platform.Extensions"))
            .ToList();

        //Assert
        stray.Should().BeEmpty();
    }
}
