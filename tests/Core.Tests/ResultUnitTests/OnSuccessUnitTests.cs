namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.OnSuccess))]
public sealed class OnSuccessUnitTests
{
    [Fact]
    public void When_Result_Is_Success_Should_Execute_Action()
    {
        // arrange
        var result = Result.Success;
        var executed = false;

        // act
        result.OnSuccess(() => executed = true);

        // assert
        executed.Should().BeTrue();
    }

    [Fact]
    public void When_Result_Is_Failed_Should_Not_Execute_Action()
    {
        // arrange
        var result = Result.Fail("TEST", "error");
        var executed = false;

        // act
        result.OnSuccess(() => executed = true);

        // assert
        executed.Should().BeFalse();
    }

    [Fact]
    public void When_Called_Should_Return_Same_Result()
    {
        // arrange
        var result = Result.Success;

        // act
        var returned = result.OnSuccess(() => { });

        // assert
        returned.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void When_Action_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange
        var result = Result.Success;

        // act | assert
        FluentActions
            .Invoking(() => result.OnSuccess(null!))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}
