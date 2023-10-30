using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class LoggingSettingsValidator : AbstractValidator<LoggingSettings>
{
    public LoggingSettingsValidator()
    {
        RuleFor(x => x.Default)
            .IsInEnum()
            .WithMessage(
                "The default log level must be one of the following: Verbose, Debug, Information, Warning, Error, Fatal"
            );
    }
}
