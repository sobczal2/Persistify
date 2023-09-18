using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Templates;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Templates;

public class CreateTemplateRequestValidatorTests
{
    private readonly IValidator<BoolField> _boolFieldValidator;
    private readonly IValidator<NumberField> _numberFieldValidator;
    private readonly ITemplateManager _templateManager;
    private readonly IValidator<TextField> _textFieldValidator;

    private CreateTemplateRequestValidator _sut;

    public CreateTemplateRequestValidatorTests()
    {
        _textFieldValidator = Substitute.For<IValidator<TextField>>();
        _numberFieldValidator = Substitute.For<IValidator<NumberField>>();
        _boolFieldValidator = Substitute.For<IValidator<BoolField>>();
        _templateManager = Substitute.For<ITemplateManager>();

        _sut = new CreateTemplateRequestValidator(_textFieldValidator, _numberFieldValidator, _boolFieldValidator,
            _templateManager);
    }

    [Fact]
    public void Ctor_WhenTextFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new CreateTemplateRequestValidator(null!, _numberFieldValidator, _boolFieldValidator,
                _templateManager);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenNumberFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new CreateTemplateRequestValidator(_textFieldValidator, null!, _boolFieldValidator,
                _templateManager);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenBoolFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new CreateTemplateRequestValidator(_textFieldValidator, _numberFieldValidator, null!,
                _templateManager);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenTemplateManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new CreateTemplateRequestValidator(_textFieldValidator, _numberFieldValidator, _boolFieldValidator,
                null!);

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
    public async Task Validate_WhenValueIsNull_ReturnsValidationException()
    {
        // Arrange

        // Act
        var result = await _sut.ValidateAsync(null!);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateTemplateRequest");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsNull_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name empty");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsTooLong_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Name too long");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenNoFields_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("No fields");
        exception.PropertyName.Should().Be("CreateTemplateRequest.*Fields");
    }

    [Fact]
    public async Task Validate_WhenTextFieldsHasOneValue_CallsTextFieldValidatorWithCorrectPropertyName()
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
            .When(x => x.ValidateAsync(Arg.Any<TextField>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should().BeEquivalentTo(new List<string> { "CreateTemplateRequest", "TextFields[0]" });
    }

    [Fact]
    public async void Validate_WhenTextFieldsValidatorReturnsValidationException_ReturnsValidationException()
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
            .ValidateAsync(Arg.Any<TextField>())
            .Returns(validationException);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public async Task Validate_WhenNumberFieldsHasOneValue_CallsNumberFieldValidatorWithCorrectPropertyName()
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
            .When(x => x.ValidateAsync(Arg.Any<NumberField>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should().BeEquivalentTo(new List<string> { "CreateTemplateRequest", "NumberFields[0]" });
    }

    [Fact]
    public async Task Validate_WhenNumberFieldsValidatorReturnsValidationException_ReturnsValidationException()
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
            .ValidateAsync(Arg.Any<NumberField>())
            .Returns(validationException);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public async Task Validate_WhenBoolFieldsHasOneValue_CallsBoolFieldValidatorWithCorrectPropertyName()
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
            .When(x => x.ValidateAsync(Arg.Any<BoolField>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should().BeEquivalentTo(new List<string> { "CreateTemplateRequest", "BoolFields[0]" });
    }

    [Fact]
    public async Task Validate_WhenBoolFieldsValidatorReturnsValidationException_ReturnsValidationException()
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
            .ValidateAsync(Arg.Any<BoolField>())
            .Returns(validationException);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public async Task Validate_WhenTextFieldsContainsDuplicateNames_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TextFields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenNumberFieldsContainsDuplicateNames_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.NumberFields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenBoolFieldsContainsDuplicateNames_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.BoolFields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenTextAndNumberFieldsContainsDuplicateNames_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.NumberFields[0].Name");
    }

    [Fact]
    public async Task Validate_WhenTextAndBoolFieldsContainsDuplicateNames_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.BoolFields[0].Name");
    }

    [Fact]
    public async Task Validate_WhenNumberAndBoolFieldsContainsDuplicateNames_ReturnsValidationException()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<ValidationException>();
        var exception = (ValidationException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.BoolFields[0].Name");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
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
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
