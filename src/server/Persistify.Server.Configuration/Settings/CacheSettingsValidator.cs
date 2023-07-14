using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class CacheSettingsValidator : AbstractValidator<CacheSettings>
{
    public CacheSettingsValidator()
    {
        RuleFor(x => x.TemplateCacheCapacity)
            .GreaterThan(0)
            .WithMessage("ManagementCacheCapacity must be greater than 0.");

        RuleFor(x => x.DocumentCacheCapacity)
            .GreaterThan(0)
            .WithMessage("DocumentCacheSize must be greater than 0.");
    }
}
