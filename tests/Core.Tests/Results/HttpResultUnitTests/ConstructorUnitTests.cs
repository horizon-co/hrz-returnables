using FluentAssertions;
using Horizon.Returnables.Core.Errors;
using Horizon.Returnables.Core.Results;
using System;
using System.Net;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Results.HttpResultUnitTests;

public sealed class ConstructorUnitTests
{
    [Fact]
    [Trait(nameof(HttpResult<object>), "new()")]
    public void When_Result_Data_Is_Invalid_An_ArgumentNullException_Should_Be_Returned()
    {
        // act
        Action withInvalidData = () =>
            new HttpResult<object>(HttpStatusCode.OK, data: default);

        // assert
        withInvalidData
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithParameterName("data");
    }

    [Fact]
    [Trait(nameof(HttpResult<object>), "new()")]
    public void When_Result_Error_Is_Invalid_An_ArgumentNullException_Should_Be_Returned()
    {
        // act
        Action withInvalidError = () =>
            new HttpResult<object>(HttpStatusCode.InternalServerError, error: default);

        // assert
        withInvalidError
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithParameterName("error");
    }

    [Fact]
    [Trait(nameof(HttpResult<object>), "new()")]
    public void When_Data_Is_Valid_Should_Return_A_HttpResult()
    {
        // act
        var result = HttpResult<object>.Create(HttpStatusCode.OK, new { });

        // assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait(nameof(HttpResult<object>), "new()")]
    public void When_Error_Is_Valid_Should_Return_A_HttpResult()
    {
        // arrange
        var error = Error.Create("SOME_CODE", "SOME_ERROR");

        // act
        var result = HttpResult<object>.Create(HttpStatusCode.BadRequest, error);

        // assert
        result.Should().NotBeNull();
        result.Data.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        result?.Error?.Code.Should().Be("SOME_CODE");
        result?.Error?.Message.Should().Be("SOME_ERROR");
    }
}