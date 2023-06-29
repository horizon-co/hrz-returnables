using FluentAssertions;
using Horizon.Returnables.Core.Errors;
using Horizon.Returnables.Core.Results;
using System;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Results.ResultUnitTests;

public sealed class ConstructorUnitTests
{
    [Fact]
    [Trait(nameof(Result<string>), "new(data)")]
    public void When_Result_Data_Is_Invalid_An_ArgumentNullException_Should_Be_Returned()
    {
        // arrange | act
        Action withDataNull = () =>
            new Result<string>(data: default);

        // assert
        withDataNull
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithParameterName("data");
    }

    [Fact]
    [Trait(nameof(Result<string>), "new(data)")]
    public void When_The_Data_Entered_Is_Valid_A_Result_Must_Be_Created_With_The_Information_Provided()
    {
        // arrange
        string data = "result data";

        // act
        var result = new Result<string>(data);

        // assert
        result.Should().NotBeNull();

        result.Success.Should().BeTrue();
        result.Error.Should().BeNull();
        result.Data.Should().Be(data);
    }

    [Fact]
    [Trait(nameof(Result<string>), "new(error)")]
    public void When_A_Null_Error_Is_Passed_An_ArgumentNullException_Should_Be_Thrown()
    {
        // arrange | act
        Action withErrorNull = () =>
            new Result<string>(error: default);

        // assert
        withErrorNull
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithParameterName("error");
    }

    [Fact]
    [Trait(nameof(Result<string>), "new(error)")]
    public void When_A_Valid_Error_Is_Passed_A_Result_Should_Be_Returned()
    {
        // arrange
        var error = Error.Create("SOME_CODE", "SOME_MESSAGE");

        // act
        var result = new Result<string>(error);

        // assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();

        result?.Error?.Code.Should().Be("SOME_CODE");
        result?.Error?.Message.Should().Be("SOME_MESSAGE");
    }
}