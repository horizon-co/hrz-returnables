namespace Hrz.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Hrz.Returnables;

using Xunit;

[Trait("Result<TValue>", nameof(Result<int>.TryAsync))]
public sealed class TryAsyncUnitTests
{
    [Fact]
    public async Task When_Async_Func_Succeeds_Should_Return_Succeeded_Result_With_Value()
    {
        // act
        var result = await Result<int>.TryAsync(async () =>
        {
            await Task.CompletedTask;
            return 42;
        });

        // assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task When_Async_Func_Throws_Should_Return_Failed_Result()
    {
        // arrange
        var exception = new InvalidOperationException("async boom");

        // act
        var result = await Result<int>.TryAsync(() => throw exception);

        // assert
        result.Failed.Should().BeTrue();
        result.Error!.Code.Should().Be("UNHANDLED");
        result.Error!.Message.Should().Be("async boom");
    }

    [Fact]
    public async Task When_Func_Is_Null_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        await FluentActions
            .Invoking(() => Result<int>.TryAsync(null!))
            .Should()
            .ThrowExactlyAsync<ArgumentNullException>();
    }
}
