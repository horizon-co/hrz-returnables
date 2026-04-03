namespace Hrz.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Hrz.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.From))]
public sealed class FromUnitTests
{
    [Fact]
    public void When_Value_Is_Provided_Should_Return_Succeeded_Result()
    {
        // act
        var result = Result<int>.From(42);

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(42);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void When_Value_Is_String_Should_Return_Succeeded_Result()
    {
        // act
        var result = Result<string>.From("hello");

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
        var result = Result<Error>.From(error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void When_Value_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        FluentActions
            .Invoking(() => Result<string>.From(null))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void When_Called_Via_Generic_Overload_Should_Delegate_To_Target_Type()
    {
        // act
        var result = Result<int>.From<string>("test");

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be("test");
    }
}
