namespace Hrz.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Hrz.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.OnSuccess))]
public sealed class OnSuccessUnitTests
{
    [Fact]
    public void When_Result_Is_Success_Should_Execute_Action_With_Value()
    {
        // arrange
        var result = Result<int>.From(42);
        int? captured = null;

        // act
        result.OnSuccess(value => captured = value);

        // assert
        captured.Should().Be(42);
    }

    [Fact]
    public void When_Result_Is_Failed_Should_Not_Execute_Action()
    {
        // arrange
        var result = Result<int>.Fail("TEST", "error");
        var executed = false;

        // act
        result.OnSuccess(_ => executed = true);

        // assert
        executed.Should().BeFalse();
    }

    [Fact]
    public void When_Called_Should_Return_Same_Result()
    {
        // arrange
        var result = Result<int>.From(42);

        // act
        var returned = result.OnSuccess(_ => { });

        // assert
        returned.Succeeded.Should().BeTrue();
        returned.Value.Should().Be(42);
    }

    [Fact]
    public void When_Action_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange
        var result = Result<int>.From(1);

        // act | assert
        FluentActions
            .Invoking(() => result.OnSuccess(null!))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}
