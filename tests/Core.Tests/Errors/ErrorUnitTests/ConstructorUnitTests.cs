using FluentAssertions;
using Horizon.Returnables.Core.Errors;
using Horizon.Returnables.Core.Errors.Enums;
using System;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Errors.ErrorUnitTests;

public sealed class ConstructorUnitTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("", null)]
    [InlineData(null, "")]
    [InlineData(null, null)]
    public void When_An_Invalid_Parameter_Is_Informed_An_Argumentnullexception_Should_Be_Thrown(string code, string message)
    {
        // arrange | act
        Action withInvalidContent = () =>
            new Error(code, message);

        // assert
        withInvalidContent
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    [Trait(nameof(Error), "new()")]
    public void When_A_Valid_Code_And_Message_Is_Informed_An_Error_Must_Be_Created()
    {
        // arrange
        var code = "400";
        var message = "erro message";

        // act
        var error = new Error(code, message);

        // assert
        error.Should().NotBeNull();
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
        error.Type.Should().Be(ErrorType.BusinessError);
    }
}