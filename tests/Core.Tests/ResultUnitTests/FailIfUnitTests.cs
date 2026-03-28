namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.FailIf))]
public sealed class FailIfUnitTests
{
    [Fact]
    public void When_Condition_Is_True_With_Message_Should_Return_Failed_Result()
    {
        // arrange
        var message = "Invalid input";

        // act
        var result = Result.FailIf(true, "TEST", message);

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Message.Should().Be(message);
    }

    [Fact]
    public void When_Condition_Is_False_With_Message_Should_Return_Success()
    {
        // act
        var result = Result.FailIf(false, "TEST", "should not appear");

        // assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void When_Condition_Is_True_With_Typed_Error_Should_Return_Failed_Result()
    {
        // arrange
        var error = Error.Create("BAD_ARG", "bad arg");

        // act
        var result = Result.FailIf(true, error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void When_Condition_Is_False_With_Typed_Error_Should_Return_Success()
    {
        // arrange
        var error = Error.Create("BAD_ARG", "bad arg");

        // act
        var result = Result.FailIf(false, error);

        // assert
        result.Succeeded.Should().BeTrue();
    }
}
