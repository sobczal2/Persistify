using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace Persistify.Client.LowLevel.Core;

public class PersistifyClientBuilder
{
    private Uri _baseAddress;
    private PersistifyCredentials _persistifyCredentials;

    private PersistifyClientBuilder()
    {
        _baseAddress = new Uri("http://localhost:5000");
        _persistifyCredentials = new PersistifyCredentials("root", "root");
    }

    public static PersistifyClientBuilder Create()
    {
        return new PersistifyClientBuilder();
    }

    public PersistifyClientBuilder WithBaseUrl(Uri baseAddress)
    {
        _baseAddress = baseAddress;
        return this;
    }

    public PersistifyClientBuilder WithCredentials(string username, string password)
    {
        _persistifyCredentials = new PersistifyCredentials(username, password);
        return this;
    }

    public IPersistifyLowLevelClient BuildLowLevel()
    {
        GrpcClientFactory.AllowUnencryptedHttp2 = true;
        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        var httpClient = new HttpClient(httpClientHandler);
        var baseUri = new Uri(_baseAddress.ToString().TrimEnd('/'));
        httpClient.BaseAddress = baseUri;

        return new PersistifyLowLevelClient(
            GrpcChannel.ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient }),
            _persistifyCredentials
        );
    }
}
