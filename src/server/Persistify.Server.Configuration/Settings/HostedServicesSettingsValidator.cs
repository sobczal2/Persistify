using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class HostedServicesSettingsValidator : AbstractValidator<HostedServicesSettings>
{
    public HostedServicesSettingsValidator()
    {
        RuleFor(x => x.TemplateManagerIntervalSeconds)
            .GreaterThan(0)
            .WithMessage("TemplateManagerIntervalSeconds must be greater than 0");
    }
}
