using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Persistify.Dtos.Response.Shared;
using Persistify.Tokenizer;

namespace Persistify.Dtos.Common.Types;

public class TypeDefinitionDto
{
    public string TypeName { get; init; } = default!;
    public FieldDefinitionDto[] Fields { get; init; } = default!;
    public string IdFieldPath { get; init; } = default!;


    public ValidationErrorDto[] Validate(string jsonString)
    {
        var errors = new List<ValidationErrorDto>();

        try
        {
            var jsonObject = JObject.Parse(jsonString);

            foreach (var field in Fields)
            {
                JToken token = jsonObject.SelectToken(field.Path);

                if (token == null)
                {
                    if (field.IsRequired)
                    {
                        errors.Add(new ValidationErrorDto
                        {
                            Field = "Data",
                            Message = $"Required field '{field.Path}' is missing."
                        });
                    }
                }
                else
                {
                    switch (field.Type)
                    {
                        case FieldTypeDto.Text:
                            if (token.Type != JTokenType.String)
                            {
                                errors.Add(TypeValidationError(field.Path, "text"));
                            }

                            break;
                        case FieldTypeDto.Number:
                            if (token.Type != JTokenType.Integer && token.Type != JTokenType.Float)
                            {
                                errors.Add(TypeValidationError(field.Path, "number"));
                            }

                            break;
                        case FieldTypeDto.Boolean:
                            if (token.Type != JTokenType.Boolean)
                            {
                                errors.Add(TypeValidationError(field.Path, "boolean"));
                            }

                            break;
                        case FieldTypeDto.Date:
                            if (token.Type != JTokenType.Date)
                            {
                                errors.Add(TypeValidationError(field.Path, "date"));
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
        catch (JsonReaderException ex)
        {
            errors.Add(new ValidationErrorDto
            {
                Field = "Data",
                Message = $"Invalid JSON: {ex.Message}"
            });
        }

        return errors.ToArray();
    }
    
    private ValidationErrorDto TypeValidationError(string path, string expectedType)
    {
        return new ValidationErrorDto
        {
            Field = "Data",
            Message = $"Field '{path}' has an invalid type. Expected {expectedType}."
        };
    }
    
    public string[] TokenizeStringFields(string data, ITokenizer tokenizer)
    {
        var jsonObject = JObject.Parse(data);

        var tokens = new List<string>();

        foreach (var field in Fields)
        {
            if (field.Type != FieldTypeDto.Text) continue;
            var token = jsonObject.SelectToken(field.Path);
            var tokenValue = token?.Value<string>();
            if (tokenValue != null)
            {
                tokens.AddRange(tokenizer.Tokenize(tokenValue));
            }
        }

        return tokens.ToArray();
    }
}