using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeBrix.Platform.Extensions;
using CodeBrix.Platform.Extensions.Collections;
using CodeBrix.Platform.Extensions.Disposables;
using CodeBrix.Platform.Extensions.Equality;
using CodeBrix.Platform.Extensions.Threading;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Platform.Extensions.Tests;

/// <summary>
/// Smoke tests that exercise representative public helpers from each vendored
/// area, confirming the bundled assembly behaves correctly end to end.
/// </summary>
public class HelperBehaviorTests
{
    //--- Disposables ---------------------------------------------------------

    [Fact]
    public void CompositeDisposable_disposes_all_children()
    {
        //Arrange
        var disposed = 0;
        var composite = new CompositeDisposable(
            Disposable.Create(() => disposed++),
            Disposable.Create(() => disposed++));

        //Act
        composite.Dispose();

        //Assert
        disposed.Should().Be(2);
    }

    [Fact]
    public void NullDisposable_instance_is_reusable_singleton()
        => NullDisposable.Instance.Should().BeSameAs(NullDisposable.Instance);

    [Fact]
    public void Disposable_Create_runs_action_once_on_dispose()
    {
        //Arrange
        var count = 0;
        var d = Disposable.Create(() => count++);

        //Act
        d.Dispose();
        d.Dispose();

        //Assert
        count.Should().Be(1);
    }

    //--- Equality ------------------------------------------------------------

    [Fact]
    public void FuncEqualityComparer_compares_by_selected_value()
    {
        //Arrange
        var comparer = FuncEqualityComparer.Create<string, int>(s => s.Length);

        //Act/Assert
        comparer.Equals("abc", "xyz").Should().BeTrue();
        comparer.Equals("abc", "wxyz").Should().BeFalse();
    }

    //--- Collections ---------------------------------------------------------

    [Fact]
    public void EnumerableExtensions_None_is_inverse_of_Any()
    {
        //Arrange
        var items = new[] { 1, 2, 3 };

        //Act/Assert
        items.None(x => x > 5).Should().BeTrue();
        items.None(x => x == 2).Should().BeFalse();
    }

    [Fact]
    public void EnumerableExtensions_ForEach_with_KeyValuePair_action_passes_index_and_item()
    {
        //Arrange
        var seen = new List<KeyValuePair<int, string>>();

        //Act
        var result = new[] { "a", "b", "c" }.ForEach(seen.Add);

        //Assert
        seen.Should().ContainInOrder(
            new KeyValuePair<int, string>(0, "a"),
            new KeyValuePair<int, string>(1, "b"),
            new KeyValuePair<int, string>(2, "c"));
        result.Should().ContainInOrder("a", "b", "c");
    }

    [Fact]
    public void EnumerableExtensions_ForEach_visits_every_item()
    {
        //Arrange
        var seen = new List<int>();

        //Act
        new[] { 1, 2, 3 }.ForEach(seen.Add).ToList();

        //Assert
        seen.Should().ContainInOrder(1, 2, 3);
    }

    //--- Extensions (strings) ------------------------------------------------

    [Fact]
    public void StringExtensions_HasValue_reflects_content()
    {
        //Arrange/Act/Assert
        "hello".HasValue().Should().BeTrue();
        "".HasValue().Should().BeFalse();
    }

    [Fact]
    public void StringExtensions_IsNullOrWhiteSpace_matches_bcl()
        => "   ".IsNullOrWhiteSpace().Should().BeTrue();

    //--- Threading -----------------------------------------------------------

    [Fact]
    public async Task FastAsyncLock_serializes_access()
    {
        //Arrange
        var gate = new FastAsyncLock();
        var log = new List<string>();

        //Act
        using (await gate.LockAsync(TestContext.Current.CancellationToken))
        {
            log.Add("in");
            log.Add("out");
        }

        // a second acquisition must succeed once the first is released
        using (await gate.LockAsync(TestContext.Current.CancellationToken))
        {
            log.Add("again");
        }

        //Assert
        log.Should().ContainInOrder("in", "out", "again");
    }
}
