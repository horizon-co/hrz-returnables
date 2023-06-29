using FluentAssertions;
using Horizon.Returnables.Core.Errors;
using Xunit;

namespace Horizon.Returnables.Core.Tests.Errors.ErrorUnitTests;

public sealed class ToStringUnitTests
{
    [Fact]
    [Trait(nameof(Error), nameof(Error.ToString))]
    public void Given_An_Error_A_String_Containing_Its_Code_And_Message_Should_Be_Returned()
    {
        // arrange
        var error = Error.Create("SOME_CODE", "SOME_MESSAGE");
        var expected = "Code: SOME_CODE\nMessage: SOME_MESSAGE\nType: BusinessError";

        // act
        var content = error.ToString();

        // assert
        content.Should().NotBeNullOrEmpty();

        content.Should().Be(expected);
    }
}