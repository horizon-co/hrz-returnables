namespace Horizon.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.OnFailure))]
public sealed class OnFailureUnitTests
{
    [Fact]
    public void When_Result_Is_Failed_Should_Execute_Action_With_Error()
    {
        // arrange
        var error = new Exception("fail");
        var result = Result<int>.Fail(error);
        Exception? captured = null;

        // act
        result.OnFailure(ex => captured = ex);

        // assert
        captured.Should().BeSameAs(error);
    }

    [Fact]
    public void When_Result_Is_Success_Should_Not_Execute_Action()
    {
        // arrange
        var result = Result<int>.From(42);
        var executed = false;

        // act
        result.OnFailure(_ => executed = true);

        // assert
        executed.Should().BeFalse();
    }

    [Fact]
    public void When_Called_Should_Return_Same_Result()
    {
        // arrange
        var result = Result<int>.Fail("error");

        // act
        var returned = result.OnFailure(_ => { });

        // assert
        returned.Failed.Should().BeTrue();
    }

    [Fact]
    public void When_Action_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange
        var result = Result<int>.Fail("error");

        // act | assert
        FluentActions
            .Invoking(() => result.OnFailure(null!))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}