namespace Persistify.Options;

public class GrpcOptions
{
    public static string SectionName => "Grpc";
    public int? MaxReceiveMessageSize { get; set; }
}