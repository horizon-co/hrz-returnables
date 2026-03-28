namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.TryAsync))]
public sealed class TryAsyncUnitTests
{
    [Fact]
    public async Task When_Async_Action_Succeeds_Should_Return_Success()
    {
        // arrange
        var executed = false;

        // act
        var result = await Result.TryAsync(async () =>
        {
            await Task.CompletedTask;
            executed = true;
        });

        // assert
        result.Succeeded.Should().BeTrue();
        executed.Should().BeTrue();
    }

    [Fact]
    public async Task When_Async_Action_Throws_Should_Return_Failed_Result()
    {
        // arrange
        var error = new InvalidOperationException("async boom");

        // act
        var result = await Result.TryAsync(() => throw error);

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public async Task When_Func_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        await FluentActions
            .Invoking(() => Result.TryAsync(null!))
            .Should()
            .ThrowExactlyAsync<ArgumentNullException>();
    }
}