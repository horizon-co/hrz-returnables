using FluentAssertions;
using Horizon.Returnables.Core.Errors;
using Horizon.Returnables.Core.Results;
using System.Net;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Results.HttpResultUnitTests;

public sealed class ConflictUnitTests
{
    [Fact]
    [Trait(nameof(HttpResult<int>), nameof(HttpResult<int>.Conflict))]
    public void Given_A_Error_An_HttpResult_Must_Be_Returned_With_StatusCode()
    {
        // arrange
        var error = Error.Create("SOME_CODE", "SOME_MESSAGE");

        // act
        var result = HttpResult<int>.Conflict(error);

        // assert
        result.Should().NotBeNull();
        result.Error.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);

        result?.Error?.Code.Should().Be("SOME_CODE");
        result?.Error?.Message.Should().Be("SOME_MESSAGE");
    }
}