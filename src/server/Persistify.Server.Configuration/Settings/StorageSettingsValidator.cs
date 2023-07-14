using System.IO;
using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class StorageSettingsValidator : AbstractValidator<StorageSettings>
{
    public StorageSettingsValidator()
    {
        RuleFor(x => x.DataPath)
            .NotEmpty()
            .WithMessage("The data path must not be empty.")
            .Must(Directory.Exists)
            .WithMessage("The data path must exist.");

        RuleFor(x => x.KeyValueStoragePath)
            .NotEmpty()
            .WithMessage("The key value storage path must not be empty.")
            .Must(Directory.Exists)
            .WithMessage("The key value storage path must exist.");
    }
}
