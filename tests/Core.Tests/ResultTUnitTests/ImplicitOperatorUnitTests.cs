namespace Horizon.Returnables.Core.Tests.ResultTUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait("Result<TValue>", "implicit operator")]
public sealed class ImplicitOperatorUnitTests
{
    [Fact]
    public void When_Error_Is_Assigned_Should_Create_Failed_Result()
    {
        // arrange
        var error = Error.Create("INVALID_OP", "fail");

        // act
        Result<int> result = error;

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void When_Succeeded_Result_Is_Converted_To_Value_Should_Return_Value()
    {
        // arrange
        var result = Result<int>.From(42);

        // act
        int value = result;

        // assert
        value.Should().Be(42);
    }

    [Fact]
    public void When_Failed_Result_Is_Converted_To_Value_Should_Return_Default()
    {
        // arrange
        var result = Result<int>.Fail("TEST", "error");

        // act
        int value = result;

        // assert
        value.Should().Be(default);
    }

    [Fact]
    public void When_Succeeded_Result_Is_Converted_To_Error_Should_Return_Null()
    {
        // arrange
        var result = Result<int>.From(42);

        // act
        Error? error = result;

        // assert
        error.Should().BeNull();
    }

    [Fact]
    public void When_Failed_Result_Is_Converted_To_Error_Should_Return_Error()
    {
        // arrange
        var expected = Error.Create("TEST", "fail");
        var result = Result<int>.Fail(expected);

        // act
        Error? error = result;

        // assert
        error.Should().BeSameAs(expected);
    }

    [Fact]
    public void When_Succeeded_Result_T_Is_Converted_To_Void_Result_Should_Be_Success()
    {
        // arrange
        var resultT = Result<int>.From(42);

        // act
        Result result = resultT;

        // assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void When_Failed_Result_T_Is_Converted_To_Void_Result_Should_Preserve_Error()
    {
        // arrange
        var error = Error.Create("TEST", "fail");
        var resultT = Result<int>.Fail(error);

        // act
        Result result = resultT;

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }
}
