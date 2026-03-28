namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.Failure))]
public sealed class FailureUnitTests
{
    [Fact]
    public void When_Accessed_Should_Return_Failed_Result()
    {
        // act
        var result = Result.Failure;

        // assert
        result.Failed.Should().BeTrue();
        result.Succeeded.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Message.Should().Be("Process failed");
    }
}