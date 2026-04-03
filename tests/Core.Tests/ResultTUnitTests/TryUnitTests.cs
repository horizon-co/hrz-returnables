namespace Hrz.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Hrz.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.Try))]
public sealed class TryUnitTests
{
    [Fact]
    public void When_Func_Succeeds_Should_Return_Succeeded_Result_With_Value()
    {
        // act
        var result = Result<int>.Try(() => 42);

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void When_Func_Throws_Should_Return_Failed_Result()
    {
        // arrange
        var exception = new InvalidOperationException("boom");

        // act
        var result = Result<int>.Try(() => throw exception);

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Code.Should().Be("UNHANDLED");
        result.Error!.Message.Should().Be("boom");
    }

    [Fact]
    public void When_Func_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        FluentActions
            .Invoking(() => Result<int>.Try(null!))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void When_Func_Returns_String_Should_Return_Succeeded_Result()
    {
        // act
        var result = Result<string>.Try(() => "hello");

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be("hello");
    }
}
