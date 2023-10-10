using System;
using FluentAssertions;
using Persistify.Helpers.Results;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Results;

public class ResultTests
{
    [Fact]
    public void Ctor_WithException_SetsException()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = new Result(exception);

        // Assert
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void Ctor_WithException_SetsFailure()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = new Result(exception);

        // Assert
        result.Failure.Should().BeTrue();
    }

    [Fact]
    public void Ctor_WithException_SetsSuccess()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = new Result(exception);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Exception_WithSuccess_ThrowsInvalidOperationException()
    {
        // Arrange
        var result = Result.Ok;

        // Act
        Action action = () => _ = result.Exception;

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Exception_WithFailure_ReturnsException()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result(exception);

        // Act
        var actual = result.Exception;

        // Assert
        actual.Should().Be(exception);
    }

    [Fact]
    public void ImplicitResult_WithException_ReturnsResult()
    {
        // Arrange
        var exception = new Exception();

        // Act
        Result result = exception;

        // Assert
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void ImplicitException_WithSuccessResult_ReturnsException()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result(exception);

        // Act
        Exception actual = result;

        // Assert
        actual.Should().Be(exception);
    }

    [Fact]
    public void ImplicitException_WithFailureResult_ThrowsInvalidOperationException()
    {
        // Arrange
        var result = Result.Ok;

        // Act
        Action action = () => _ = (Exception)result;

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void OnSuccess_WithSuccess_CallsAction()
    {
        // Arrange
        var actionCalled = false;
        var result = Result.Ok;

        // Act
        result.OnSuccess(() => actionCalled = true);

        // Assert
        actionCalled.Should().BeTrue();
    }

    [Fact]
    public void OnSuccess_WithFailure_DoesNotCallAction()
    {
        // Arrange
        var actionCalled = false;
        var result = new Result(new Exception());

        // Act
        result.OnSuccess(() => actionCalled = true);

        // Assert
        actionCalled.Should().BeFalse();
    }

    [Fact]
    public void OnFailure_WithSuccess_DoesNotCallAction()
    {
        // Arrange
        var actionCalled = false;
        var result = Result.Ok;

        // Act
        result.OnFailure(_ => actionCalled = true);

        // Assert
        actionCalled.Should().BeFalse();
    }

    [Fact]
    public void OnFailure_WithFailure_CallsAction()
    {
        // Arrange
        var actionCalled = false;
        var exception = new Exception();
        var result = new Result(exception);

        // Act
        result.OnFailure(_ => actionCalled = true);

        // Assert
        actionCalled.Should().BeTrue();
    }

    [Fact]
    public void Match_WithSuccess_CallsOnSuccess()
    {
        // Arrange
        var onSuccessCalled = false;
        var onFailureCalled = false;
        var result = Result.Ok;

        // Act
        result.Match(() => onSuccessCalled = true, _ => onFailureCalled = true);

        // Assert
        onSuccessCalled.Should().BeTrue();
        onFailureCalled.Should().BeFalse();
    }

    [Fact]
    public void Match_WithFailure_CallsOnFailure()
    {
        // Arrange
        var onSuccessCalled = false;
        var onFailureCalled = false;
        var exception = new Exception();
        var result = new Result(exception);

        // Act
        result.Match(() => onSuccessCalled = true, _ => onFailureCalled = true);

        // Assert
        onSuccessCalled.Should().BeFalse();
        onFailureCalled.Should().BeTrue();
    }
}
