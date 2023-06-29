using FluentAssertions;
using Horizon.Returnables.Core.Results;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Results.HttpResultUnitTests;

public sealed class NoContentUnitTests
{
    [Fact]
    [Trait(nameof(HttpResult<int>), nameof(HttpResult<int>.NoContent))]
    public void A_HttpResult_Must_Be_Returned_With_StatusCode()
    {
        // act
        var result = HttpResult<int>.NoContent();

        // assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
    }
}