using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class HostedServicesSettingsValidator : AbstractValidator<HostedServicesSettings>
{
    public HostedServicesSettingsValidator()
    {
        RuleFor(x => x.DocumentManagerIntervalSeconds)
            .GreaterThan(0)
            .WithMessage("DocumentManagerRecurrentActionIntervalInSeconds must be greater than 0");
    }
}
