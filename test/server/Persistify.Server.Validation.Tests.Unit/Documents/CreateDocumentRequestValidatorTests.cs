using System;
using System.Collections.Generic;
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
        Action act = () =>
        {
            var unused = new CreateDocumentRequestValidator(
                null!,
                _numberFieldValueValidator,
                _boolFieldValueValidator
            );
        };

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
    public void Ctor_WhenCorrect_SetsPropertyNamesCorrectly()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "CreateDocumentRequest" });
        _textFieldValueValidator.Received().PropertyName = _sut.PropertyName;
        _numberFieldValueValidator.Received().PropertyName = _sut.PropertyName;
        _boolFieldValueValidator.Received().PropertyName = _sut.PropertyName;
    }

    [Fact]
    public void Validate_WhenValueIsNull_ReturnsValidationException()
    {
        // Arrange

        // Act
        var result = _sut.Validate(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateDocumentRequest");
    }

    [Fact]
    public void Validate_WhenTemplateIdIsZero_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest { TemplateId = 0 };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Invalid template id");
        exception.PropertyName.Should().Be("CreateDocumentRequest.TemplateId");
    }

    [Fact]
    public void Validate_WhenNoFieldValues_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest { TemplateId = 1 };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("No field values");
        exception.PropertyName.Should().Be("CreateDocumentRequest.*FieldValues");
    }

    [Fact]
    public void Validate_WhenTextFieldValuesHasOneValue_CallsTextFieldValueValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1, TextFieldValues = new List<TextFieldValue> { new() }
        };
        List<string> propertyNameAtCall = null!;
        _textFieldValueValidator
            .When(x => x.Validate(Arg.Any<TextFieldValue>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "TextFieldValues[0]" }));
    }

    [Fact]
    public void Validate_WhenTextFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1, TextFieldValues = new List<TextFieldValue> { new() }
        };
        var validationException = new ValidationException("Test", "Test");
        _textFieldValueValidator
            .Validate(Arg.Any<TextFieldValue>())
            .Returns(validationException);

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public void Validate_WhenNumberFieldValuesHasOneValue_CallsNumberFieldValueValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1, NumberFieldValues = new List<NumberFieldValue> { new() }
        };
        List<string> propertyNameAtCall = null!;
        _numberFieldValueValidator
            .When(x => x.Validate(Arg.Any<NumberFieldValue>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "NumberFieldValues[0]" }));
    }

    [Fact]
    public void Validate_WhenNumberFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1, NumberFieldValues = new List<NumberFieldValue> { new() }
        };
        var validationException = new ValidationException("Test", "Test");
        _numberFieldValueValidator
            .Validate(Arg.Any<NumberFieldValue>())
            .Returns(validationException);

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public void Validate_WhenBoolFieldValuesHasOneValue_CallsBoolFieldValueValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1, BoolFieldValues = new List<BoolFieldValue> { new() }
        };

        List<string> propertyNameAtCall = null!;
        _boolFieldValueValidator
            .When(x => x.Validate(Arg.Any<BoolFieldValue>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "BoolFieldValues[0]" }));
    }

    [Fact]
    public void Validate_WhenBoolFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1, BoolFieldValues = new List<BoolFieldValue> { new() }
        };
        var validationException = new ValidationException("Test", "Test");
        _boolFieldValueValidator
            .Validate(Arg.Any<BoolFieldValue>())
            .Returns(validationException);

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public void Validate_WhenTextFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1,
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" }, new() { FieldName = "1" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.TextFieldValues[1].FieldName");
    }

    [Fact]
    public void Validate_WhenNumberFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1,
            NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "1" }, new() { FieldName = "1" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.NumberFieldValues[1].FieldName");
    }

    [Fact]
    public void Validate_WhenBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1,
            BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "1" }, new() { FieldName = "1" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.BoolFieldValues[1].FieldName");
    }

    [Fact]
    public void Validate_WhenTextAndNumberFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1,
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" } },
            NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "1" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.NumberFieldValues[0].FieldName");
    }

    [Fact]
    public void Validate_WhenTextAndBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1,
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" } },
            BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "1" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.BoolFieldValues[0].FieldName");
    }

    [Fact]
    public void Validate_WhenNumberAndBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateId = 1,
            NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "1" } },
            BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "1" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.BoolFieldValues[0].FieldName");
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var value = new CreateDocumentRequest
        {
            TemplateId = 1,
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" } },
            NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "2" } },
            BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "3" } }
        };

        // Act
        var result = _sut.Validate(value);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
