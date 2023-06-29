using FluentAssertions;
using Horizon.Returnables.Core.Errors;
using Horizon.Returnables.Core.Results;
using System;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Results.ResultUnitTests;

public sealed class CreateUnitTests
{
    [Fact]
    [Trait(nameof(Result<string>), nameof(Result<string>.Create))]
    public void When_Result_Data_Is_Invalid_An_ArgumentNullException_Should_Be_Returned()
    {
        // arrange | act
        Action withInvalidData = () =>
            Result<string>.Create(result: default);

        // assert
        withInvalidData
            .Should()
            .Throw<ArgumentNullException>()
            .WithParameterName("data");
    }

    [Fact]
    [Trait(nameof(Result<string>), nameof(Result<string>.Create))]
    public void When_The_Data_Entered_Is_Valid_A_Result_Must_Be_Created_With_The_Information_Provided()
    {
        // arrange | act
        string result = Result<string>.Create("SOME_DATA_CONTENT");

        // assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("SOME_DATA_CONTENT");
    }

    [Fact]
    [Trait(nameof(Result<string>), nameof(Result<string>.Create))]
    public void When_A_Null_Error_Is_Passed_An_ArgumentNullException_Should_Be_Thrown()
    {
        // arrange | act
        Action withInvalidError = () =>
            Result<string>.Create(error: default);

        // assert
        withInvalidError
            .Should()
            .ThrowExactly<ArgumentNullException>()
            .WithParameterName("error");
    }

    [Fact]
    [Trait(nameof(Result<string>), nameof(Result<string>.Create))]
    public void When_A_Valid_Error_Is_Informed_A_Result_Should_Be_Returned()
    {
        // act
        Error error = Result<string>.Create(Error.Create("SOME_CODE", "SOME_MESSAGE"));

        // assert
        error.Should().NotBeNull();
        error.Code.Should().Be("SOME_CODE");
        error.Message.Should().Be("SOME_MESSAGE");
    }
}