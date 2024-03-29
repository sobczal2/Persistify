﻿namespace Persistify.Server.ErrorHandling.ErrorMessages;

public static class TemplateErrorMessages
{
    public const string NameEmpty = "Name empty";

    public const string AnalyzerPresetNameOrAnalyzerDescriptorRequired =
        "Analyzer preset name or analyzer descriptor required";

    public const string NoFields = "No fields";
    public const string FieldNameNotUnique = "Field name not unique";
    public const string NameNotUnique = "Name not unique";
    public const string TemplateNotFound = "Template not found";
    public const string PresetNotFound = "Preset not found";
    public const string InvalidField = "Invalid field";
    public const string PresetAnalyzerNotFound = "Preset analyzer not found";
    public const string InvalidAnalyzerDescriptor = "Invalid analyzer descriptor";
    public const string TemplateAlreadyExists = "Template already exists";
}
