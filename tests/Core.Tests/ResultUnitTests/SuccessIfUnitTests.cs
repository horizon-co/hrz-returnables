namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.SuccessIf))]
public sealed class SuccessIfUnitTests
{
    [Fact]
    public void When_Condition_Is_True_With_Message_Should_Return_Success()
    {
        // act
        var result = Result.SuccessIf(true, "TEST", "should not appear");

        // assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void When_Condition_Is_False_With_Message_Should_Return_Failed_Result()
    {
        // arrange
        var message = "Condition not met";

        // act
        var result = Result.SuccessIf(false, "TEST", message);

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Message.Should().Be(message);
    }

    [Fact]
    public void When_Condition_Is_True_With_Typed_Error_Should_Return_Success()
    {
        // arrange
        var error = Error.Create("DENIED", "denied");

        // act
        var result = Result.SuccessIf(true, error);

        // assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void When_Condition_Is_False_With_Typed_Error_Should_Return_Failed_Result()
    {
        // arrange
        var error = Error.Create("DENIED", "denied");

        // act
        var result = Result.SuccessIf(false, error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }
}
