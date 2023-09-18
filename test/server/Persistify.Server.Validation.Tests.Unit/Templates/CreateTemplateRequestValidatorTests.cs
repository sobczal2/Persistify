using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Templates;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Templates;

public class CreateTemplateRequestValidatorTests
{
    private readonly IValidator<BoolField> _boolFieldValidator;
    private readonly IValidator<NumberField> _numberFieldValidator;
    private CreateTemplateRequestValidator _sut;

    private readonly IValidator<TextField> _textFieldValidator;

    public CreateTemplateRequestValidatorTests()
    {
        _textFieldValidator = Substitute.For<IValidator<TextField>>();
        _numberFieldValidator = Substitute.For<IValidator<NumberField>>();
        _boolFieldValidator = Substitute.For<IValidator<BoolField>>();

        _sut = new CreateTemplateRequestValidator(_textFieldValidator, _numberFieldValidator, _boolFieldValidator);
    }

    [Fact]
    public void Ctor_WhenTextFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new CreateTemplateRequestValidator(null!, _numberFieldValidator, _boolFieldValidator);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenNumberFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new CreateTemplateRequestValidator(_textFieldValidator, null!, _boolFieldValidator);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenBoolFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () => _sut = new CreateTemplateRequestValidator(_textFieldValidator, _numberFieldValidator, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenCorrect_SetsPropertyNamesCorrectly()
    {
        // Arrange

        // Act

        // Assert
        _sut.PropertyName.Should().BeEquivalentTo(new List<string> { "CreateTemplateRequest" });
        _textFieldValidator.Received().PropertyName = _sut.PropertyName;
        _numberFieldValidator.Received().PropertyName = _sut.PropertyName;
        _boolFieldValidator.Received().PropertyName = _sut.PropertyName;
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
        exception.PropertyName.Should().Be("CreateTemplateRequest");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = null!,
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField>()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = string.Empty,
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField>()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenTemplateNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = new string('a', 65),
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField>()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name too long");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TemplateName");
    }

    [Fact]
    public void Validate_WhenNoFields_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField>()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("No fields");
        exception.PropertyName.Should().Be("CreateTemplateRequest.*Fields");
    }

    [Fact]
    public void Validate_WhenTextFieldsHasOneValue_CallsTextFieldValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField> { new() },
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField>()
        };
        List<string> propertyNameAtCall = null!;
        _textFieldValidator
            .When(x => x.Validate(Arg.Any<TextField>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should().BeEquivalentTo(new List<string> { "CreateTemplateRequest", "TextFields[0]" });
    }

    [Fact]
    public void Validate_WhenTextFieldsValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField> { new() },
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField>()
        };
        var validationException = new ValidationException("Test", "Test");
        _textFieldValidator
            .Validate(Arg.Any<TextField>())
            .Returns(validationException);

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public void Validate_WhenNumberFieldsHasOneValue_CallsNumberFieldValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField> { new() },
            BoolFields = new List<BoolField>()
        };
        List<string> propertyNameAtCall = null!;
        _numberFieldValidator
            .When(x => x.Validate(Arg.Any<NumberField>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should().BeEquivalentTo(new List<string> { "CreateTemplateRequest", "NumberFields[0]" });
    }

    [Fact]
    public void Validate_WhenNumberFieldsValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField> { new() },
            BoolFields = new List<BoolField>()
        };
        var validationException = new ValidationException("Test", "Test");
        _numberFieldValidator
            .Validate(Arg.Any<NumberField>())
            .Returns(validationException);

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public void Validate_WhenBoolFieldsHasOneValue_CallsBoolFieldValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField> { new() }
        };
        List<string> propertyNameAtCall = null!;
        _boolFieldValidator
            .When(x => x.Validate(Arg.Any<BoolField>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        _sut.Validate(request);

        // Assert
        propertyNameAtCall.Should().BeEquivalentTo(new List<string> { "CreateTemplateRequest", "BoolFields[0]" });
    }

    [Fact]
    public void Validate_WhenBoolFieldsValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField> { new() }
        };
        var validationException = new ValidationException("Test", "Test");
        _boolFieldValidator
            .Validate(Arg.Any<BoolField>())
            .Returns(validationException);

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public void Validate_WhenTextFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField> { new() { Name = "name" }, new() { Name = "name" } },
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField>()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TextFields[1].Name");
    }

    [Fact]
    public void Validate_WhenNumberFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField> { new() { Name = "name" }, new() { Name = "name" } },
            BoolFields = new List<BoolField>()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.NumberFields[1].Name");
    }

    [Fact]
    public void Validate_WhenBoolFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField> { new() { Name = "name" }, new() { Name = "name" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.BoolFields[1].Name");
    }

    [Fact]
    public void Validate_WhenTextAndNumberFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField> { new() { Name = "name" } },
            NumberFields = new List<NumberField> { new() { Name = "name" } },
            BoolFields = new List<BoolField>()
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.NumberFields[0].Name");
    }

    [Fact]
    public void Validate_WhenTextAndBoolFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField> { new() { Name = "name" } },
            NumberFields = new List<NumberField>(),
            BoolFields = new List<BoolField> { new() { Name = "name" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.BoolFields[0].Name");
    }

    [Fact]
    public void Validate_WhenNumberAndBoolFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField>(),
            NumberFields = new List<NumberField> { new() { Name = "name" } },
            BoolFields = new List<BoolField> { new() { Name = "name" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.BoolFields[0].Name");
    }

    [Fact]
    public void Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            TextFields = new List<TextField> { new() { Name = "name" } },
            NumberFields = new List<NumberField> { new() { Name = "name2" } },
            BoolFields = new List<BoolField> { new() { Name = "name3" } }
        };

        // Act
        var result = _sut.Validate(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
