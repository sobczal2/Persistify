using System;
using FluentAssertions;
using NSubstitute;
using Persistify.Domain.Documents;
using Persistify.Requests.Documents;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Documents;

public class CreateDocumentRequestValidatorTests
{
    private readonly CreateDocumentRequestValidator _sut;

    private readonly IValidator<TextFieldValue> _textFieldValueValidator;
    private readonly IValidator<NumberFieldValue> _numberFieldValueValidator;
    private readonly IValidator<BoolFieldValue> _boolFieldValueValidator;

    public CreateDocumentRequestValidatorTests()
    {
        _textFieldValueValidator = Substitute.For<IValidator<TextFieldValue>>();
        _numberFieldValueValidator = Substitute.For<IValidator<NumberFieldValue>>();
        _boolFieldValueValidator = Substitute.For<IValidator<BoolFieldValue>>();

        _sut = new CreateDocumentRequestValidator(
            _textFieldValueValidator,
            _numberFieldValueValidator,
            _boolFieldValueValidator
        );
    }

    [Fact]
    public void Ctor_WhenTextFieldValueValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => new CreateDocumentRequestValidator(
            null!,
            _numberFieldValueValidator,
            _boolFieldValueValidator
        );

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenNumberFieldValueValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => new CreateDocumentRequestValidator(
            _textFieldValueValidator,
            null!,
            _boolFieldValueValidator
        );

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenBoolFieldValueValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => new CreateDocumentRequestValidator(
            _textFieldValueValidator,
            _numberFieldValueValidator,
            null!
        );

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Validate_WhenTemplateIdIsZero_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 0
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.OnFailure(x => x.Message.Should().Be("Invalid template id"));
    }
}
