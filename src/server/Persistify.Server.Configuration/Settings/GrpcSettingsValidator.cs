using FluentValidation;

namespace Persistify.Server.Configuration.Settings;

public class GrpcSettingsValidator : AbstractValidator<GrpcSettings>
{
    public GrpcSettingsValidator()
    {
        RuleFor(x => x.ResponseCompressionLevel)
            .Matches(@"^(Optimal|Fastest|NoCompression|SmallestSize)$")
            .WithMessage("The compression level must be either Optimal, Fastest, NoCompression or SmallestSize");

        RuleFor(x => x.ResponseCompressionAlgorithm)
            .Matches(@"^(gzip|Identity)$")
            .WithMessage("The compression algorithm must be either gzip or Identity.");

        RuleFor(x => x.MaxReceiveMessageSize)
            .GreaterThan(0)
            .WithMessage("The max receive message size must be greater than 0.");

        RuleFor(x => x.MaxSendMessageSize)
            .GreaterThan(0)
            .WithMessage("The max send message size must be greater than 0.");
    }
}
