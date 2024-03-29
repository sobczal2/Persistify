﻿using System;
using System.Threading.Tasks;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
using Persistify.Helpers.Results;
using Persistify.Server.ErrorHandling.ErrorMessages;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Validation.Dtos.Fields;

public class TextFieldDtoValidator : Validator<TextFieldDto>
{
    private readonly IValidator<FullAnalyzerDto> _fullAnalyzerDtoValidator;
    private readonly IValidator<PresetNameAnalyzerDto> _presetNameDescriptorDtoValidator;

    public TextFieldDtoValidator(
        IValidator<FullAnalyzerDto> fullAnalyzerDtoValidator,
        IValidator<PresetNameAnalyzerDto> presetNameDescriptorDtoValidator
    )
    {
        _fullAnalyzerDtoValidator = fullAnalyzerDtoValidator ??
                                    throw new ArgumentNullException(nameof(fullAnalyzerDtoValidator));
        _fullAnalyzerDtoValidator.PropertyName = PropertyName;
        _presetNameDescriptorDtoValidator = presetNameDescriptorDtoValidator ??
                                            throw new ArgumentNullException(
                                                nameof(presetNameDescriptorDtoValidator)
                                            );
        _presetNameDescriptorDtoValidator.PropertyName = PropertyName;
        PropertyName.Push(nameof(TextFieldDto));
    }

    public override async ValueTask<Result> ValidateNotNullAsync(
        TextFieldDto value
    )
    {
        if (string.IsNullOrEmpty(value.Name))
        {
            PropertyName.Push(nameof(TextFieldDto.Name));
            return StaticValidationException(TemplateErrorMessages.NameEmpty);
        }

        if (value.Name.Length > 64)
        {
            PropertyName.Push(nameof(TextFieldDto.Name));
            return StaticValidationException(SharedErrorMessages.ValueTooLong);
        }

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (value.AnalyzerDto is null && value.IndexFullText)
        {
            PropertyName.Push(nameof(TextFieldDto.AnalyzerDto));
            return StaticValidationException(SharedErrorMessages.ValueNull);
        }

        if (value.AnalyzerDto is not null && !value.IndexFullText)
        {
            PropertyName.Push(nameof(TextFieldDto.IndexFullText));
            return StaticValidationException(SharedErrorMessages.InvalidValue);
        }

        if (value.AnalyzerDto is not null)
        {
            if (value.AnalyzerDto is PresetNameAnalyzerDto presetNameAnalyzerDto)
            {
                PropertyName.Push(nameof(TextFieldDto.AnalyzerDto));
                var presetNameAnalyzerDtoValidationResult =
                    await _presetNameDescriptorDtoValidator.ValidateAsync(presetNameAnalyzerDto);
                PropertyName.Pop();
                if (presetNameAnalyzerDtoValidationResult.Failure)
                {
                    return presetNameAnalyzerDtoValidationResult;
                }
            }
            else if (value.AnalyzerDto is FullAnalyzerDto fullAnalyzerDto)
            {
                PropertyName.Push(nameof(TextFieldDto.AnalyzerDto));
                var fullAnalyzerDtoValidator = await _fullAnalyzerDtoValidator.ValidateAsync(
                    fullAnalyzerDto
                );
                PropertyName.Pop();
                if (fullAnalyzerDtoValidator.Failure)
                {
                    return fullAnalyzerDtoValidator;
                }
            }
        }

        return Result.Ok;
    }
}
