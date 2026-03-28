namespace Horizon.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.Fail))]
public sealed class FailUnitTests
{
    [Fact]
    public void When_A_Message_Is_Provided_Should_Create_Failed_Result()
    {
        // arrange
        var message = "Something went wrong";

        // act
        var result = Result<int>.Fail(message);

        // assert
        result.Failed.Should().BeTrue();
        result.Succeeded.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Message.Should().Be(message);
        result.Value.Should().Be(default(int));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void When_Message_Is_Null_Or_Whitespace_Should_Return_Failed_Result_With_Default_Message(string? message)
    {
        // act
        var result = Result<string>.Fail(message);

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
        var result = Result<string>.Fail(error);

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
            .Invoking(() => Result<string>.Fail<InvalidOperationException>(null))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void When_Called_Without_Arguments_Should_Return_Failed_Result_With_Default_Message()
    {
        // act
        var result = Result<int>.Fail();

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Message.Should().Be("Process failed");
    }
}