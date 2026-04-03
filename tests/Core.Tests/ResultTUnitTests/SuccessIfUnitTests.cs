namespace Horizon.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.SuccessIf))]
public sealed class SuccessIfUnitTests
{
    [Fact]
    public void When_Condition_Is_True_With_Message_Should_Return_Success_With_Value()
    {
        // act
        var result = Result<int>.SuccessIf(true, 42, "TEST", "error");

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void When_Condition_Is_False_With_Message_Should_Return_Failed_Result()
    {
        // arrange
        var message = "Must be valid";

        // act
        var result = Result<int>.SuccessIf(false, 42, "TEST", message);

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
        var result = Result<string>.SuccessIf(true, "ok", error);

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public void When_Condition_Is_False_With_Typed_Error_Should_Return_Failed_Result()
    {
        // arrange
        var error = Error.Create("DENIED", "denied");

        // act
        var result = Result<string>.SuccessIf(false, "ok", error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }
}
