namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.Fail))]
public sealed class FailUnitTests
{
    [Fact]
    public void When_A_Message_Is_Provided_Should_Create_Failed_Result()
    {
        // arrange
        var message = "Something went wrong";

        // act
        var result = Result.Fail(message);

        // assert
        result.Failed.Should().BeTrue();
        result.Succeeded.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Message.Should().Be(message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void When_Message_Is_Null_Or_Whitespace_Should_Return_Failed_Result_With_Default_Message(string? message)
    {
        // act
        var result = Result.Fail(message);

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Message.Should().Be("Process failed");
    }

    [Fact]
    public void When_Typed_Exception_Is_Provided_Should_Preserve_Exception_Type()
    {
        // arrange
        var error = new InvalidOperationException("Invalid state");

        // act
        var result = Result.Fail(error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
        result.Error.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void When_Typed_Exception_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        FluentActions
            .Invoking(() => Result.Fail<InvalidOperationException>(null))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void When_Called_Without_Arguments_Should_Return_Failed_Result_With_Default_Message()
    {
        // act
        var result = Result.Fail();

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Message.Should().Be("Process failed");
    }

    [Fact]
    public void When_Exception_Has_Inner_Exception_Should_Preserve_Full_Chain()
    {
        // arrange
        var inner = new ArgumentException("Bad argument");
        var error = new InvalidOperationException("Outer error", inner);

        // act
        var result = Result.Fail(error);

        // assert
        result.Error!.InnerException.Should().BeSameAs(inner);
    }
}