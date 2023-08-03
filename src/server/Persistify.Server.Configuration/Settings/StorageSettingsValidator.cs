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

        RuleFor(x => x.StorageType)
            .IsInEnum()
            .WithMessage("The storage type must be a valid value.");

        RuleFor(x => x.SerializerType)
            .IsInEnum()
            .WithMessage("The serializer type must be a valid value.");

        RuleFor(x => x.RepositorySectorSize)
            .GreaterThanOrEqualTo(64)
            .WithMessage("The repository sector size must be greater than or equal to 64.");
    }
}
