namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.From))]
public sealed class FromUnitTests
{
    [Fact]
    public void When_Value_Is_Provided_Should_Return_Succeeded_Result()
    {
        // arrange
        var value = 42;

        // act
        var result = Result.From(value);

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void When_Value_Is_String_Should_Return_Succeeded_Result()
    {
        // arrange
        var value = "hello";

        // act
        var result = Result.From(value);

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void When_Value_Is_Error_Should_Return_Failed_Result()
    {
        // arrange
        var error = Error.Create("INVALID_OP", "fail");

        // act
        var result = Result.From<Error>(error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void When_Value_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        FluentActions
            .Invoking(() => Result.From<string>(null))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}
