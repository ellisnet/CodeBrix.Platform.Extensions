using System.Reflection;
using CodeBrix.Platform.Extensions.Disposables;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Platform.Extensions.Tests;

/// <summary>
/// Confirms representative types from each vendored area resolve in their
/// renamed CodeBrix.Platform.Extensions.* namespaces.
/// </summary>
public class NamespaceMappingTests
{
    private static Assembly Lib => typeof(Disposable).Assembly;

    [Theory]
    [InlineData("CodeBrix.Platform.Extensions.Disposables.CompositeDisposable")]
    [InlineData("CodeBrix.Platform.Extensions.NullDisposable")]
    [InlineData("CodeBrix.Platform.Extensions.Threading.FastAsyncLock")]
    [InlineData("CodeBrix.Platform.Extensions.Equality.FuncEqualityComparer")]
    [InlineData("CodeBrix.Platform.Extensions.StringExtensions")]
    [InlineData("CodeBrix.Platform.Extensions.EnumerableExtensions")]
    [InlineData("CodeBrix.Platform.Extensions.Logging.LogExtensions")]
    public void Renamed_type_is_present(string fullName)
        => Lib.GetType(fullName).Should().NotBeNull();

    [Theory]
    [InlineData("Uno.Disposables.CompositeDisposable")]
    [InlineData("Uno.Threading.FastAsyncLock")]
    [InlineData("Uno.Extensions.StringExtensions")]
    public void Original_uno_type_is_gone(string fullName)
        => Lib.GetType(fullName).Should().BeNull();
}
