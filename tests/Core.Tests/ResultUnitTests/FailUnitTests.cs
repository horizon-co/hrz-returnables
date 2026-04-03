namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.Fail))]
public sealed class FailUnitTests
{
    [Fact]
    public void When_Code_And_Message_Are_Provided_Should_Create_Failed_Result()
    {
        // arrange
        var code = "TEST";
        var message = "Something went wrong";

        // act
        var result = Result.Fail(code, message);

        // assert
        result.Failed.Should().BeTrue();
        result.Succeeded.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(code);
        result.Error!.Message.Should().Be(message);
    }


    [Theory]
    [InlineData(null, "message")]
    [InlineData("CODE", null)]
    public void When_Code_Or_Message_Is_Null_Should_Throw_ArgumentNullException(string? code, string? message)
    {
        // act | assert
        FluentActions
            .Invoking(() => Result.Fail(code, message))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
    [Theory]
    [InlineData("", "message")]
    [InlineData("   ", "message")]
    [InlineData("CODE", "")]
    [InlineData("CODE", "   ")]
    public void When_Code_Or_Message_Whitespace_Should_Throw_ArgumentException(string? code, string? message)
    {
        // act | assert
        FluentActions
            .Invoking(() => Result.Fail(code, message))
            .Should()
            .ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void When_Typed_Error_Is_Provided_Should_Preserve_Error_Instance()
    {
        // arrange
        var error = Error.Create("INVALID_OP", "Invalid state");

        // act
        var result = Result.Fail(error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
        result.Error!.Code.Should().Be("INVALID_OP");
    }

    [Fact]
    public void When_Typed_Error_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        FluentActions
            .Invoking(() => Result.Fail<Error>(null))
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
}