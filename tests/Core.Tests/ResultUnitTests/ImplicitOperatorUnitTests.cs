namespace Horizon.Returnables.Core.Tests.ResultUnitTests;

using FluentAssertions;

using Horizon.Returnables;

using Xunit;

[Trait(nameof(Result), "implicit operator")]
public sealed class ImplicitOperatorUnitTests
{
    [Fact]
    public void When_Exception_Is_Assigned_Should_Create_Failed_Result()
    {
        // arrange
        var error = new InvalidOperationException("fail");

        // act
        Result result = error;

        // assert
        result.Failed.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void When_Success_Result_Is_Converted_To_Exception_Should_Return_Null()
    {
        // arrange
        var result = Result.Success;

        // act
        Exception? error = result;

        // assert
        error.Should().BeNull();
    }

    [Fact]
    public void When_Failed_Result_Is_Converted_To_Exception_Should_Return_Error()
    {
        // arrange
        var expected = new Exception("fail");
        var result = Result.Fail(expected);

        // act
        Exception? error = result;

        // assert
        error.Should().BeSameAs(expected);
    }

    [Fact]
    public void When_Null_Exception_Is_Assigned_Should_Throw_ArgumentNullException()
    {
        // arrange | act | assert
        FluentActions
            .Invoking(() => { Result result = (Exception?)null; })
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }
}