using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;

namespace Persistify.Client.LowLevel.Core;

public class PersistifyClientBuilder
{
    private Uri _baseAddress;
    private PersistifyCredentials _persistifyCredentials;
    private ConnectionSettings _connectionSettings;

    private PersistifyClientBuilder()
    {
        _baseAddress = new Uri("http://localhost:5000");
        _persistifyCredentials = new PersistifyCredentials("root", "root");
        _connectionSettings = ConnectionSettings.NoTls;
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

    public PersistifyClientBuilder WithConnectionSettings(ConnectionSettings connectionSettings)
    {
        _connectionSettings = connectionSettings;
        return this;
    }

    public IPersistifyLowLevelClient BuildLowLevel()
    {
        var httpClient = new HttpClient { DefaultRequestVersion = new Version(2, 0) };

        switch (_connectionSettings)
        {
            case ConnectionSettings.NoTls:
                break;

            case ConnectionSettings.TlsVerify:
                httpClient = new HttpClient(new HttpClientHandler());
                break;

            case ConnectionSettings.TlsNoVerify:
                httpClient = new HttpClient(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                });
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(_connectionSettings), "Unsupported connection settings.");
        }

        return new PersistifyLowLevelClient(
            GrpcChannel.ForAddress(_baseAddress, new GrpcChannelOptions { HttpClient = httpClient }),
            _persistifyCredentials
        );
    }
}
