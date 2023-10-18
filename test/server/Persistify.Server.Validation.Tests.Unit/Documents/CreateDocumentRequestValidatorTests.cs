using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Persistify.Domain.Documents;
using Persistify.Domain.Templates;
using Persistify.Dtos.Documents.FieldValues;
using Persistify.Requests.Documents;
using Persistify.Server.ErrorHandling.Exceptions;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Validation.Common;
using Persistify.Server.Validation.Requests.Documents;
using Xunit;

namespace Persistify.Server.Validation.Tests.Unit.Documents;

public class CreateDocumentRequestValidatorTests
{
    private readonly IValidator<BoolFieldValueDto> _boolFieldValueValidator;
    private readonly IValidator<NumberFieldValueDto> _numberFieldValueValidator;

    private readonly CreateDocumentRequestValidator _sut;
    private readonly ITemplateManager _templateManager;
    private readonly IValidator<TextFieldValueDto> _textFieldValueValidator;


    public CreateDocumentRequestValidatorTests()
    {
        _textFieldValueValidator = Substitute.For<IValidator<TextFieldValueDto>>();
        _numberFieldValueValidator = Substitute.For<IValidator<NumberFieldValueDto>>();
        _boolFieldValueValidator = Substitute.For<IValidator<BoolFieldValueDto>>();
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
            FieldValues = new List<FieldValueDto> { new TextFieldValueDto { FieldName = "Test", Value = "Test" } }
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
        exception.Message.Should().Be("Value null");
        exception.PropertyName.Should().Be("CreateDocumentRequest.FieldValues");
    }

    [Fact]
    public async Task Validate_WhenTextFieldValuesHasOneValue_CallsTextFieldValueValidatorWithCorrectPropertyName()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new TextFieldValueDto { FieldName = "Test" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            Fields = new List<Field>()
        });
        List<string> propertyNameAtCall = null!;
        _textFieldValueValidator
            .When(x => x.ValidateAsync(Arg.Any<TextFieldValueDto>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "FieldValues[0]" }));
    }

    [Fact]
    public async Task Validate_WhenTextFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", FieldValues = new List<FieldValueDto> { new TextFieldValueDto() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _textFieldValueValidator
            .ValidateAsync(Arg.Any<TextFieldValueDto>())
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
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new NumberFieldValueDto { FieldName = "test" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template() { Fields = new List<Field>() });
        List<string> propertyNameAtCall = null!;
        _numberFieldValueValidator
            .When(x => x.ValidateAsync(Arg.Any<NumberFieldValueDto>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "FieldValues[0]" }));
    }

    [Fact]
    public async Task Validate_WhenNumberFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", FieldValues = new List<FieldValueDto> { new NumberFieldValueDto() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _numberFieldValueValidator
            .ValidateAsync(Arg.Any<NumberFieldValueDto>())
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
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new BoolFieldValueDto() { FieldName = "test" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template()
        {
            Fields = new List<Field>()
        });
        List<string> propertyNameAtCall = null!;
        _boolFieldValueValidator
            .When(x => x.ValidateAsync(Arg.Any<BoolFieldValueDto>()))
            .Do(x => propertyNameAtCall = new List<string>(_sut.PropertyName));

        // Act
        await _sut.ValidateAsync(request);

        // Assert
        propertyNameAtCall.Should()
            .BeEquivalentTo(new List<string>(new[] { "CreateDocumentRequest", "FieldValues[0]" }));
    }

    [Fact]
    public async Task Validate_WhenBoolFieldValueValidatorReturnsValidationException_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test", FieldValues = new List<FieldValueDto> { new BoolFieldValueDto() }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());
        var validationException = new StaticValidationPersistifyException("Test", "Test");
        _boolFieldValueValidator
            .ValidateAsync(Arg.Any<BoolFieldValueDto>())
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
            FieldValues = new List<FieldValueDto>
            {
                new TextFieldValueDto { FieldName = "1" }, new TextFieldValueDto { FieldName = "1" }
            }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template()
        {
            Fields = new List<Field>()
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.FieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenNumberFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto>
            {
                new NumberFieldValueDto { FieldName = "1" }, new NumberFieldValueDto { FieldName = "1" }
            }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.FieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto>
            {
                new BoolFieldValueDto { FieldName = "1" }, new BoolFieldValueDto { FieldName = "1" }
            }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.FieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenTextAndNumberFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto>
            {
                new TextFieldValueDto { FieldName = "1" }, new NumberFieldValueDto { FieldName = "1" }
            },
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.FieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenTextAndBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto>
            {
                new TextFieldValueDto { FieldName = "1" }, new BoolFieldValueDto { FieldName = "1" }
            },
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.FieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenNumberAndBoolFieldValuesContainsDuplicateFieldNames_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto>
            {
                new NumberFieldValueDto { FieldName = "1" }, new BoolFieldValueDto { FieldName = "1" }
            },
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template());

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeTrue();
        result.Exception.Should().BeOfType<DynamicValidationPersistifyException>();
        var exception = (PersistifyException)result.Exception;
        exception.Message.Should().Be("Field name not unique");
        exception.PropertyName.Should().Be("CreateDocumentRequest.FieldValues[1].FieldName");
    }

    [Fact]
    public async Task Validate_WhenRequiredTextFieldIsMissing_ReturnsValidationException()
    {
        // Arrange
        var request = new CreateDocumentRequest
        {
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new TextFieldValueDto { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            Fields = new List<Field>
            {
                new TextField
                {
                    Analyzer = new Analyzer
                    {
                        CharacterFilterNames = new List<string>(),
                        TokenizerName = "test",
                        TokenFilterNames = new List<string>()
                    },
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
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new NumberFieldValueDto { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            Fields = new List<Field> { new NumberField { Name = "2", Required = true } }
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
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new BoolFieldValueDto { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            Fields = new List<Field> { new BoolField { Name = "2", Required = true } }
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
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new TextFieldValueDto { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            Fields = new List<Field>
            {
                new TextField
                {
                    Analyzer = new Analyzer
                    {
                        CharacterFilterNames = new List<string>(),
                        TokenizerName = "test",
                        TokenFilterNames = new List<string>()
                    },
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
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new NumberFieldValueDto { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            Fields = new List<Field> { new NumberField { Name = "2", Required = false } }
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
            TemplateName = "Test",
            FieldValues = new List<FieldValueDto> { new BoolFieldValueDto { FieldName = "1" } }
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            Fields = new List<Field> { new BoolField { Name = "2", Required = false } }
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
            FieldValues = new List<FieldValueDto>
            {
                new TextFieldValueDto { FieldName = "1" },
                new NumberFieldValueDto { FieldName = "2" },
                new BoolFieldValueDto { FieldName = "3" }
            },
        };
        _templateManager.GetAsync(request.TemplateName).Returns(new Template
        {
            Fields =
                new List<Field>
                {
                    new TextField
                    {
                        Analyzer = new Analyzer()
                        {
                            CharacterFilterNames = new List<string>(),
                            TokenizerName = "test",
                            TokenFilterNames = new List<string>()
                        },
                        Name = "1",
                        Required = true
                    },
                    new NumberField { Name = "2", Required = true },
                    new BoolField { Name = "3", Required = true }
                },
        });

        // Act
        var result = await _sut.ValidateAsync(request);

        // Assert
        result.Failure.Should().BeFalse();
    }
}
