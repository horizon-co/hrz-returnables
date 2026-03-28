namespace Horizon.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.Switch))]
public sealed class SwitchUnitTests
{
    [Fact]
    public void When_Result_Is_Success_Should_Invoke_OnSuccess_With_Value()
    {
        // arrange
        var result = Result<int>.From(42);
        int? captured = null;

        // act
        result.Switch(
            onSuccess: value => captured = value,
            onFailure: _ => { });

        // assert
        captured.Should().Be(42);
    }

    [Fact]
    public void When_Result_Is_Failed_Should_Invoke_OnFailure_With_Error()
    {
        // arrange
        var error = new Exception("fail");
        var result = Result<int>.Fail(error);
        Exception? captured = null;

        // act
        result.Switch(
            onSuccess: _ => { },
            onFailure: ex => captured = ex);

        // assert
        captured.Should().BeSameAs(error);
    }

    [Fact]
    public void When_OnSuccess_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange
        var result = Result<int>.From(1);

        // act | assert
        FluentActions
            .Invoking(() => result.Switch(null!, _ => { }))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void When_OnFailure_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange
        var result = Result<int>.From(1);

        // act | assert
        FluentActions
            .Invoking(() => result.Switch(_ => { }, null!))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}