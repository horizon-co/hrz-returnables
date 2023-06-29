using FluentAssertions;
using Horizon.Returnables.Core.Results;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Results.HttpResultUnitTests;

public sealed class CreatedUnitTests
{
    [Fact]
    [Trait(nameof(HttpResult<int>), nameof(HttpResult<int>.Create))]
    public void Given_A_Result_An_HttpResult_Must_Be_Returned_With_StatusCode()
    {
        // act
        var result = HttpResult<int>.Create(1);

        // assert
        result.Should().NotBeNull();
        result.Data.Should().Be(1);
        result.Error.Should().BeNull();
        result.Success.Should().BeTrue();
    }
}