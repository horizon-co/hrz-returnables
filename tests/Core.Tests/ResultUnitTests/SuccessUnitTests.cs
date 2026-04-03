namespace Hrz.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Hrz.Returnables;

using Xunit;

[Trait(nameof(Result), nameof(Result.Success))]
public sealed class SuccessUnitTests
{
    [Fact]
    public void When_Accessed_Should_Return_Succeeded_Result()
    {
        // act
        var result = Result.Success;

        // assert
        result.Succeeded.Should().BeTrue();
        result.Failed.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void When_Constructed_With_Default_Should_Be_Succeeded()
    {
        // act
        var result = new Result();

        // assert
        result.Succeeded.Should().BeTrue();
        result.Error.Should().BeNull();
    }
}