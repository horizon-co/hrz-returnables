namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.Switch))]
public sealed class SwitchUnitTests
{
    [Fact]
    public void When_Result_Is_Success_Should_Invoke_OnSuccess()
    {
        // arrange
        var result = Result.Success;
        var successCalled = false;

        // act
        result.Switch(
            onSuccess: () => successCalled = true,
            onFailure: _ => { });

        // assert
        successCalled.Should().BeTrue();
    }

    [Fact]
    public void When_Result_Is_Failed_Should_Invoke_OnFailure()
    {
        // arrange
        var error = new Exception("fail");
        var result = Result.Fail(error);
        Exception? captured = null;

        // act
        result.Switch(
            onSuccess: () => { },
            onFailure: ex => captured = ex);

        // assert
        captured.Should().BeSameAs(error);
    }

    [Fact]
    public void When_OnSuccess_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange
        var result = Result.Success;

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
        var result = Result.Success;

        // act | assert
        FluentActions
            .Invoking(() => result.Switch(() => { }, null!))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}