namespace Persistify.Server.Configuration.Settings;

public class GrpcSettings
{
    public static string SectionName => "Grpc";

    public bool EnableDetailedErrors { get; set; }
    public string ResponseCompressionLevel { get; set; } = default!;
    public string ResponseCompressionAlgorithm { get; set; } = default!;
    public int MaxReceiveMessageSize { get; set; }
    public int MaxSendMessageSize { get; set; }
    public bool IgnoreUnknownServices { get; set; }
}
