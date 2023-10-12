using FluentAssertions;
using Persistify.Server.ErrorHandling.Exceptions;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Common;

public class ValidationExceptionTests
{
    [Fact]
    public void Ctor_WithPropertyAndMessage_ShouldSetPropertyAndMessage()
    {
        // Arrange
        var property = "property";
        var message = "message";

        // Act
        var result = new PersistifyException(property, message);

        // Assert
        result.PropertyName.Should().Be(property);
        result.Message.Should().Be(message);
    }
}
