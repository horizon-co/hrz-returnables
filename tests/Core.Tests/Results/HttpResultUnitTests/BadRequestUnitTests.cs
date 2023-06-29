using FluentAssertions;
using Horizon.Returnables.Core.Errors;
using Horizon.Returnables.Core.Results;
using System.Net;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Results.HttpResultUnitTests;

public sealed class BadRequestUnitTests
{
    [Fact]
    [Trait(nameof(HttpResult<int>), nameof(HttpResult<int>.BadRequest))]
    public void Given_A_Error_An_HttpResult_Must_Be_Returned_With_StatusCode()
    {
        // arrange
        var error = Error.Create("SOME_CODE", "SOME_MESSAGE");

        // act
        var result = HttpResult<int>.BadRequest(error);

        // assert
        result.Should().NotBeNull();
        result.Error.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result?.Error?.Code.Should().Be("SOME_CODE");
        result?.Error?.Message.Should().Be("SOME_MESSAGE");
    }
}