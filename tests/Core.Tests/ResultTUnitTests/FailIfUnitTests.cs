namespace Horizon.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.FailIf))]
public sealed class FailIfUnitTests
{
    [Fact]
    public void When_Condition_Is_True_With_Message_Should_Return_Failed_Result()
    {
        // arrange
        var message = "Invalid input";

        // act
        var result = Result<int>.FailIf(true, 42, message);

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Message.Should().Be(message);
    }

    [Fact]
    public void When_Condition_Is_False_With_Message_Should_Return_Success_With_Value()
    {
        // act
        var result = Result<int>.FailIf(false, 42, "should not appear");

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void When_Condition_Is_True_With_Typed_Exception_Should_Return_Failed_Result()
    {
        // arrange
        var error = new ArgumentException("bad arg");

        // act
        var result = Result<string>.FailIf(true, "value", error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void When_Condition_Is_False_With_Typed_Exception_Should_Return_Success()
    {
        // arrange
        var error = new ArgumentException("bad arg");

        // act
        var result = Result<string>.FailIf(false, "value", error);

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be("value");
    }
}