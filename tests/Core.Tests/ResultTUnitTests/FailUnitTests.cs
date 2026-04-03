namespace Hrz.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Hrz.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.Fail))]
public sealed class FailUnitTests
{
    [Fact]
    public void When_Code_And_Message_Are_Provided_Should_Create_Failed_Result()
    {
        // arrange
        var code = "TEST";
        var message = "Something went wrong";

        // act
        var result = Result<int>.Fail(code, message);

        // assert
        result.Failed.Should().BeTrue();
        result.Succeeded.Should().BeFalse();
        result.Error.Should().NotBeNull();
        result.Error!.Code.Should().Be(code);
        result.Error!.Message.Should().Be(message);
        result.Value.Should().Be(default(int));
    }

    [Theory]
    [InlineData(null, "message")]
    [InlineData("code", null)]
    public void When_Code_Or_Message_Is_Null_Should_Throw_ArgumentNullException(string? code, string? message)
    {
        // act | assert
        FluentActions
            .Invoking(() => Result<string>.Fail(code, message))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Theory]
    [InlineData("", "message")]
    [InlineData("   ", "message")]
    [InlineData("CODE", "")]
    [InlineData("CODE", "   ")]
    public void When_Code_Or_Message_Is_Whitespace_Should_Throw_ArgumentException(string? code, string? message)
    {
        // act | assert
        FluentActions
            .Invoking(() => Result<string>.Fail(code, message))
            .Should()
            .ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void When_Typed_Error_Is_Provided_Should_Preserve_Error()
    {
        // arrange
        var error = Error.Create("INVALID_OP", "Invalid state");

        // act
        var result = Result<string>.Fail(error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void When_Typed_Error_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        FluentActions
            .Invoking(() => Result<string>.Fail<Error>(null))
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