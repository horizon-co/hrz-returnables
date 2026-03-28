namespace Horizon.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.Failure))]
public sealed class FailureUnitTests
{
    [Fact]
    public void When_Accessed_Should_Return_Failed_Result()
    {
        // act
        var result = Result<int>.Failure;

        // assert
        result.Failed.Should().BeTrue();
        result.Succeeded.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Message.Should().Be("Process failed");
    }

    [Fact]
    public void When_Accessed_With_Reference_Type_Should_Have_Null_Value()
    {
        // act
        var result = Result<string>.Failure;

        // assert
        result.Failed.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}