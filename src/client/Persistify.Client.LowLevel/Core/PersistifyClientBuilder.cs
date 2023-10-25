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
        return new PersistifyLowLevelClient(
            _baseAddress,
            _persistifyCredentials
        );
    }
}
