using FluentAssertions;
using Horizon.Returnables.Core.Results;
using System.Net;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Results.HttpResultUnitTests;

public sealed class OKUnitTests
{
    [Fact]
    [Trait(nameof(HttpResult<int>), nameof(HttpResult<int>.OK))]
    public void Given_A_Result_An_HttpResult_Must_Be_Returned_With_StatusCode()
    {
        // act
        var result = HttpResult<int>.OK(1);

        // assert
        result.Should().NotBeNull();
        result.Data.Should().Be(1);
        result.Error.Should().BeNull();
        result.Success.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}