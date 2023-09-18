using System;
using FluentAssertions;
using Persistify.Server.Validation.Results;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Results;

public class ResultTTests
{
    [Fact]
    public void Ctor_WithValue_SetsValue()
    {
        // Arrange
        var value = new object();

        // Act
        var result = new Result<object>(value);

        // Assert
        result.Value.Should().Be(value);
    }

    [Fact]
    public void Ctor_WithValue_SetsFailure()
    {
        // Arrange
        var value = new object();

        // Act
        var result = new Result<object>(value);

        // Assert
        result.Failure.Should().BeFalse();
    }

    [Fact]
    public void Ctor_WithValue_SetsSuccess()
    {
        // Arrange
        var value = new object();

        // Act
        var result = new Result<object>(value);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public void Ctor_WithException_SetsException()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = new Result<object>(exception);

        // Assert
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void Ctor_WithException_SetsFailure()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = new Result<object>(exception);

        // Assert
        result.Failure.Should().BeTrue();
    }

    [Fact]
    public void Ctor_WithException_SetsSuccess()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = new Result<object>(exception);

        // Assert
        result.Success.Should().BeFalse();
    }

    [Fact]
    public void Value_WithFailure_ThrowsInvalidOperationException()
    {
        // Arrange
        var result = new Result<object>(new Exception());

        // Act
        Action action = () => _ = result.Value;

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Value_WithSuccess_ReturnsValue()
    {
        // Arrange
        var value = new object();
        var result = new Result<object>(value);

        // Act
        var actual = result.Value;

        // Assert
        actual.Should().Be(value);
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
        var result = new Result<object>(exception);

        // Act
        var actual = result.Exception;

        // Assert
        actual.Should().Be(exception);
    }

    [Fact]
    public void ImplicitResultTFromValue_WithValue_ReturnsResultT()
    {
        // Arrange
        var value = new object();

        // Act
        Result<object> result = value;

        // Assert
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ImplicitResultTFromException_WithException_ReturnsResultT()
    {
        // Arrange
        var exception = new Exception();

        // Act
        Result<object> result = exception;

        // Assert
        result.Exception.Should().Be(exception);
    }

    [Fact]
    public void ImplicitValue_WithSuccessResultT_ReturnsValue()
    {
        // Arrange
        var value = new TestClassForResult();
        var result = new Result<TestClassForResult>(value);

        // Act
        TestClassForResult actual = result;

        // Assert
        actual.Should().Be(value);
    }

    [Fact]
    public void ImplicitValue_WithFailureResultT_ThrowsInvalidOperationException()
    {
        // Arrange
        var result = new Result<TestClassForResult>(new Exception());

        // Act
        Action action = () => _ = (TestClassForResult)result;

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitException_WithSuccessResultT_ThrowsInvalidOperationException()
    {
        // Arrange
        var result = Result.Ok;

        // Act
        Action action = () => _ = (Exception)result;

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitException_WithFailureResultT_ReturnsException()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<object>(exception);

        // Act
        Exception actual = result;

        // Assert
        actual.Should().Be(exception);
    }

    [Fact]
    public void OnSuccess_WithSuccess_CallsAction()
    {
        // Arrange
        var value = new object();
        var result = new Result<object>(value);
        var called = false;

        // Act
        result.OnSuccess(_ => called = true);

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public void OnSuccess_WithFailure_DoesNotCallAction()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<object>(exception);
        var called = false;

        // Act
        result.OnSuccess(_ => called = true);

        // Assert
        called.Should().BeFalse();
    }

    [Fact]
    public void OnFailure_WithSuccess_DoesNotCallAction()
    {
        // Arrange
        var value = new object();
        var result = new Result<object>(value);
        var called = false;

        // Act
        result.OnFailure(_ => called = true);

        // Assert
        called.Should().BeFalse();
    }

    [Fact]
    public void OnFailure_WithFailure_CallsAction()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<object>(exception);
        var called = false;

        // Act
        result.OnFailure(_ => called = true);

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public void Match_WithSuccess_ReturnsOnSuccess()
    {
        // Arrange
        var value = new object();
        var result = new Result<object>(value);
        var expected = new object();

        // Act
        var actual = result.Match(
            _ => expected,
            _ => new object());

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Match_WithFailure_ReturnsOnFailure()
    {
        // Arrange
        var exception = new Exception();
        var result = new Result<object>(exception);
        var expected = new object();

        // Act
        var actual = result.Match(
            _ => new object(),
            _ => expected);

        // Assert
        actual.Should().Be(expected);
    }

    private class TestClassForResult
    {
    }
}
