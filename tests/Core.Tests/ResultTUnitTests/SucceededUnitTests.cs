namespace Horizon.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait("Result<TValue>", "Succeeded")]
public sealed class SucceededUnitTests
{
    [Fact]
    public void When_Result_Has_Value_Should_Be_True()
    {
        // arrange
        var result = Result<int>.From(42);

        // assert
        result.Succeeded.Should().BeTrue();
        result.Failed.Should().BeFalse();
    }

    [Fact]
    public void When_Result_Has_Error_Should_Be_False()
    {
        // arrange
        var result = Result<int>.Fail("TEST", "error");

        // assert
        result.Succeeded.Should().BeFalse();
        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void When_Succeeded_Value_Should_Not_Be_Null()
    {
        // arrange
        var result = Result<string>.From("hello");

        // act | assert
        if (result.Succeeded)
        {
            result.Value.Should().NotBeNull();
            result.Error.Should().BeNull();
        }
    }

    [Fact]
    public void When_Failed_Error_Should_Not_Be_Null()
    {
        // arrange
        var result = Result<string>.Fail("TEST", "error");

        // act | assert
        if (result.Failed)
        {
            result.Error.Should().NotBeNull();
        }
    }
}
