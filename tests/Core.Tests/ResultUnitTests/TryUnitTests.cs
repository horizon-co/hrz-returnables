namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.Try))]
public sealed class TryUnitTests
{
    [Fact]
    public void When_Action_Succeeds_Should_Return_Success()
    {
        // arrange
        var executed = false;

        // act
        var result = Result.Try(() => executed = true);

        // assert
        result.Succeeded.Should().BeTrue();
        executed.Should().BeTrue();
    }

    [Fact]
    public void When_Action_Throws_Should_Return_Failed_Result()
    {
        // arrange
        var exception = new InvalidOperationException("boom");

        // act
        var result = Result.Try(() => throw exception);

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Code.Should().Be("UNHANDLED");
        result.Error!.Message.Should().Be("boom");
    }

    [Fact]
    public void When_Action_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        FluentActions
            .Invoking(() => Result.Try(null!))
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}
