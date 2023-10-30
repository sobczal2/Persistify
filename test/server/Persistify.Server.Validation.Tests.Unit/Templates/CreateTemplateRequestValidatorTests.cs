using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Dtos.Templates.Fields;
using Persistify.Requests.Templates;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Requests.Templates;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Templates;

public class CreateTemplateRequestValidatorTests
{
    private readonly IValidator<BoolFieldDto> _boolFieldValidator;
    private readonly IValidator<NumberFieldDto> _numberFieldValidator;
    private readonly ITemplateManager _templateManager;
    private readonly IValidator<TextFieldDto> _textFieldValidator;

    private CreateTemplateRequestValidator _sut;

    public CreateTemplateRequestValidatorTests()
    {
        _textFieldValidator = Substitute.For<IValidator<TextFieldDto>>();
        _numberFieldValidator = Substitute.For<IValidator<NumberFieldDto>>();
        _boolFieldValidator = Substitute.For<IValidator<BoolFieldDto>>();
        _templateManager = Substitute.For<ITemplateManager>();

        _sut = new CreateTemplateRequestValidator(
            _textFieldValidator,
            _numberFieldValidator,
            _boolFieldValidator,
            _templateManager
        );
    }

    [Fact]
    public void Ctor_WhenTextFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new CreateTemplateRequestValidator(
                null!,
                _numberFieldValidator,
                _boolFieldValidator,
                _templateManager
            );

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenNumberFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new CreateTemplateRequestValidator(
                _textFieldValidator,
                null!,
                _boolFieldValidator,
                _templateManager
            );

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenBoolFieldValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new CreateTemplateRequestValidator(
                _textFieldValidator,
                _numberFieldValidator,
                null!,
                _templateManager
            );

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WhenTemplateManagerIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        Action act = () =>
            _sut = new CreateTemplateRequestValidator(
                _textFieldValidator,
                _numberFieldValidator,
                _boolFieldValidator,
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
        _sut.PropertyName
            .Should()
            .BeEquivalentTo(new List<string> { "CreateTemplateRequest" });
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
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
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
            Fields = new List<FieldDto>()
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
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
            Fields = new List<FieldDto>()
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
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
            Fields = new List<FieldDto>()
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("CreateTemplateRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenNoFields_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto>()
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("No fields");
        exception.PropertyName.Should().Be("CreateTemplateRequest.Fields");
    }

    [Fact]
    public async Task Validate_WhenTextFieldsHasOneValue_CallsTextFieldValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto> { new TextFieldDto() }
        };
        List<string> propertyNameAtCall = null!;
        _textFieldValidator
            .When(x => x.ValidateAsync(Arg.Any<TextFieldDto>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall
            .Should()
            .BeEquivalentTo(new List<string> { "CreateTemplateRequest", "Fields[0]" });
    }

    [Fact]
    public async void Validate_WhenTextFieldsValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto> { new TextFieldDto() }
        };
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _textFieldValidator.ValidateAsync(Arg.Any<TextFieldDto>()).Returns(validationException);

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
            Fields = new List<FieldDto> { new NumberFieldDto() }
        };
        List<string> propertyNameAtCall = null!;
        _numberFieldValidator
            .When(x => x.ValidateAsync(Arg.Any<NumberFieldDto>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall
            .Should()
            .BeEquivalentTo(new List<string> { "CreateTemplateRequest", "Fields[0]" });
    }

    [Fact]
    public async Task Validate_WhenNumberFieldsValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto> { new NumberFieldDto() }
        };
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _numberFieldValidator.ValidateAsync(Arg.Any<NumberFieldDto>()).Returns(validationException);

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
            Fields = new List<FieldDto> { new BoolFieldDto() }
        };
        List<string> propertyNameAtCall = null!;
        _boolFieldValidator
            .When(x => x.ValidateAsync(Arg.Any<BoolFieldDto>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall
            .Should()
            .BeEquivalentTo(new List<string> { "CreateTemplateRequest", "Fields[0]" });
    }

    [Fact]
    public async Task Validate_WhenBoolFieldsValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto> { new BoolFieldDto() }
        };
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _boolFieldValidator.ValidateAsync(Arg.Any<BoolFieldDto>()).Returns(validationException);

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
            Fields = new List<FieldDto>
            {
                new TextFieldDto { Name = "name" },
                new TextFieldDto { Name = "name" }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.Fields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenNumberFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto>
            {
                new NumberFieldDto { Name = "name" },
                new NumberFieldDto { Name = "name" }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.Fields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenBoolFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto>
            {
                new BoolFieldDto { Name = "name" },
                new BoolFieldDto { Name = "name" }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.Fields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenTextAndNumberFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto>
            {
                new TextFieldDto { Name = "name" },
                new NumberFieldDto { Name = "name" }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.Fields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenTextAndBoolFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto>
            {
                new TextFieldDto { Name = "name" },
                new BoolFieldDto { Name = "name" }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.Fields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenNumberAndBoolFieldsContainsDuplicateNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto>
            {
                new NumberFieldDto { Name = "name" },
                new BoolFieldDto { Name = "name" }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateTemplateRequest.Fields[1].Name");
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new CreateTemplateRequest
        {
            TemplateName = "template",
            Fields = new List<FieldDto>
            {
                new TextFieldDto { Name = "name1" },
                new NumberFieldDto { Name = "name2" },
                new BoolFieldDto { Name = "name3" }
            }
        };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
