using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Requests.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Documents;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Documents;

public class CreateDocumentRequestValidatorTests
{
    private readonly IValidator<BoolFieldValue> _boolFieldValueValidator;
    private readonly IValidator<NumberFieldValue> _numberFieldValueValidator;

    private readonly CreateDocumentRequestValidator _sut;
    private readonly ITemplateManager _templateManager;
    private readonly IValidator<TextFieldValue> _textFieldValueValidator;


    public CreateDocumentRequestValidatorTests()
    {
        _textFieldValueValidator = Substitute.For<IValidator<TextFieldValue>>();
        _numberFieldValueValidator = Substitute.For<IValidator<NumberFieldValue>>();
        _boolFieldValueValidator = Substitute.For<IValidator<BoolFieldValue>>();
        _templateManager = Substitute.For<ITemplateManager>();

        _sut = new CreateDocumentRequestValidator(
            _textFieldValueValidator,
            _numberFieldValueValidator,
            _boolFieldValueValidator,
            _templateManager
        );
    }

    [Fact]
    public void Ctor_WhenTextFieldValueValidatorIsNull_ThrowsArgumentNullException()
    {
        // Arrange

        // Act
        var act = () =>
        {
            var unused = new CreateDocumentRequestValidator(
                null!,
                _numberFieldValueValidator,
                _boolFieldValueValidator,
                _templateManager
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
            _boolFieldValueValidator,
            _templateManager
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
        Action act = () => new CreateDocumentRequestValidator(
            _textFieldValueValidator,
            _numberFieldValueValidator,
            _boolFieldValueValidator,
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
        exception.PropertyName.Should().Be("CreateDocumentRequest");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsNull_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest();

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsEmpty_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest { TemplateName = string.Empty };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateNameIsTooLong_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest { TemplateName = new string('a', 65) };

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Value too long");
        exception.PropertyName.Should().Be("CreateDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenTemplateDoesNotExist_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "Test", Value = "Test" } }
        };
        _templateManager.Exists(request.TemplateName).Returns(false);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Template not found");
        exception.PropertyName.Should().Be("CreateDocumentRequest.TemplateName");
    }

    [Fact]
    public async Task Validate_WhenNoFieldValues_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest { TemplateName = "Test" };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<StaticValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("No field values");
        exception.PropertyName.Should().Be("CreateDocumentRequest.*FieldValues");
    }

    [Fact]
    public async Task Validate_WhenTextFieldValuesHasOneValue_CallsTextFieldValueValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", TextFieldValues = new List<TextFieldValue> { new() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        List<string> propertyNameAtCall = null!;
        _textFieldValueValidator
            .When(x => x.ValidateAsync(Arg.Any<TextFieldValue>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "TextFieldValues[0]" }));
    }

    [Fact]
    public async Task Validate_WhenTextFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", TextFieldValues = new List<TextFieldValue> { new() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _textFieldValueValidator
            .ValidateAsync(Arg.Any<TextFieldValue>())
            .Returns(validationException);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public async Task Validate_WhenNumberFieldValuesHasOneValue_CallsNumberFieldValueValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", NumberFieldValues = new List<NumberFieldValue> { new() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        List<string> propertyNameAtCall = null!;
        _numberFieldValueValidator
            .When(x => x.ValidateAsync(Arg.Any<NumberFieldValue>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "NumberFieldValues[0]" }));
    }

    [Fact]
    public async Task Validate_WhenNumberFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", NumberFieldValues = new List<NumberFieldValue> { new() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _numberFieldValueValidator
            .ValidateAsync(Arg.Any<NumberFieldValue>())
            .Returns(validationException);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public async Task Validate_WhenBoolFieldValuesHasOneValue_CallsBoolFieldValueValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", BoolFieldValues = new List<BoolFieldValue> { new() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        List<string> propertyNameAtCall = null!;
        _boolFieldValueValidator
            .When(x => x.ValidateAsync(Arg.Any<BoolFieldValue>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "BoolFieldValues[0]" }));
    }

    [Fact]
    public async Task Validate_WhenBoolFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", BoolFieldValues = new List<BoolFieldValue> { new() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _boolFieldValueValidator
            .ValidateAsync(Arg.Any<BoolFieldValue>())
            .Returns(validationException);

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().Be(validationException);
    }

    [Fact]
    public async Task Validate_WhenTextFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" }, new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.TextFieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenNumberFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "1" }, new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.NumberFieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "1" }, new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.BoolFieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenTextAndNumberFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" } },
            NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.NumberFieldValues[0].FieldName");
    }

    [Fact]
    public async Task Validate_WhenTextAndBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" } },
            BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.BoolFieldValues[0].FieldName");
    }

    [Fact]
    public async Task Validate_WhenNumberAndBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "1" } },
            BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.BoolFieldValues[0].FieldName");
    }

    [Fact]
    public async Task Validate_WhenRequiredTextFieldIsMissing_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            TextFields = new List<TextField>
            {
                new()
                {
                    AnalyzerDescriptor = new PresetAnalyzerDescriptor { PresetName = "test" },
                    Name = "2",
                    Required = true
                }
            }
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
    }

    [Fact]
    public async Task Validate_WhenRequiredNumberFieldIsMissing_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            NumberFields = new List<NumberField> { new() { Name = "2", Required = true } }
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
    }

    [Fact]
    public async Task Validate_WhenRequiredBoolFieldIsMissing_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            BoolFields = new List<BoolField> { new() { Name = "2", Required = true } }
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
    }

    [Fact]
    public async Task Validate_WhenNotRequiredTextFieldIsMissing_ReturnsOk()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            TextFields = new List<TextField>
            {
                new()
                {
                    AnalyzerDescriptor = new PresetAnalyzerDescriptor { PresetName = "test" },
                    Name = "2",
                    Required = false
                }
            }
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WhenNotRequiredNumberFieldIsMissing_ReturnsOk()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            NumberFields = new List<NumberField> { new() { Name = "2", Required = false } }
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WhenNotRequiredBoolFieldIsMissing_ReturnsOk()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            BoolFields = new List<BoolField> { new() { Name = "2", Required = false } }
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            TextFieldValues = new List<TextFieldValue> { new() { FieldName = "1" } },
            NumberFieldValues = new List<NumberFieldValue> { new() { FieldName = "2" } },
            BoolFieldValues = new List<BoolFieldValue> { new() { FieldName = "3" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            TextFields =
                new List<TextField>
                {
                    new() { AnalyzerDescriptor = new PresetAnalyzerDescriptor { PresetName = "test" }, Name = "1" }
                },
            NumberFields = new List<NumberField> { new() { Name = "2" } },
            BoolFields = new List<BoolField> { new() { Name = "3" } }
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
