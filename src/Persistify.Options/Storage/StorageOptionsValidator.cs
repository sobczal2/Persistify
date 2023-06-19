using FluentValidation;

namespace Persistify.Options.Storage;

public class StorageOptionsValidator : AbstractValidator<StorageOptions>
{
    public StorageOptionsValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(x => x is "Files" or "GzipFiles" or "InMemory")
            .WithMessage("Storage type must be one of: Files, GzipFiles, InMemory");
        
        RuleFor(x => x.Path)
            .NotEmpty()
            .When(x => x.Type is "Files" or "GzipFiles")
            .WithMessage("Storage url must be specified for Files or GzipFiles storage type");
    }
}