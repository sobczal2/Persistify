using Grpc.Core;
using Grpc.Net.Client;
using Persistify.Client.Documents;
using Persistify.Client.PresetAnalyzers;
using Persistify.Client.Templates;
using Persistify.Client.Users;
using Persistify.Requests.Users;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

namespace Persistify.Client.Core;

public class PersistifyClient : IPersistifyClient, IDisposable
{
    internal PersistifyClient(Uri baseAddress, PersistifyCredentials persistifyCredentials)
    {
        GrpcClientFactory.AllowUnencryptedHttp2 = true;
        Channel = GrpcChannel.ForAddress(baseAddress);
        PersistifyCredentials = persistifyCredentials;
        Users = new UsersClient(this);
        Templates = new TemplatesClient(this);
        Documents = new DocumentsClient(this);
        PresetAnalyzerses = new PresetAnalyzersClient(this);
    }

    private PersistifyCredentials PersistifyCredentials { get; }
    internal GrpcChannel Channel { get; }

    public void Dispose()
    {
        Channel.Dispose();

        GC.SuppressFinalize(this);
    }

    public IUsersClient Users { get; }
    public ITemplatesClient Templates { get; }
    public IDocumentsClient Documents { get; }
    public IPresetAnalyzersClient PresetAnalyzerses { get; }

    internal async Task<TResponse> CallAuthenticatedServiceAsync<TResponse>(
        Func<CallContext, Task<TResponse>> serviceCall,
        CallContext? callContext = null
    )
    {
        callContext ??= new CallContext(new CallOptions(new Metadata()));

        if (callContext.Value.RequestHeaders == null)
        {
            throw new Exception("Request headers are null");
        }

        if (string.IsNullOrEmpty(PersistifyCredentials.AccessToken))
        {
            await AuthenticateAsync(callContext);
        }

        SetAuthorizationHeader(callContext.Value);

        try
        {
            return await serviceCall(callContext.Value);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
        {
            await HandleUnauthenticatedExceptionAsync(callContext);
            SetAuthorizationHeader(callContext.Value);
            return await serviceCall(callContext.Value);
        }
    }

    private async Task AuthenticateAsync(CallContext? callContext)
    {
        var signInResponse = await this.SignInAsync(
            new SignInRequest { Username = PersistifyCredentials.Username, Password = PersistifyCredentials.Password },
            callContext);

        PersistifyCredentials.AccessToken = signInResponse.AccessToken;
        PersistifyCredentials.RefreshToken = signInResponse.RefreshToken;
    }

    private async Task HandleUnauthenticatedExceptionAsync(CallContext? callContext)
    {
        try
        {
            if (string.IsNullOrEmpty(PersistifyCredentials.RefreshToken))
            {
                throw new Exception("Refresh token is null or empty");
            }

            var refreshTokenResponse = await this.RefreshTokenAsync(
                new RefreshTokenRequest { RefreshToken = PersistifyCredentials.RefreshToken }, callContext);
            PersistifyCredentials.AccessToken = refreshTokenResponse.AccessToken;
            PersistifyCredentials.RefreshToken = refreshTokenResponse.RefreshToken;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
        {
            await AuthenticateAsync(callContext);
        }
    }

    private void SetAuthorizationHeader(CallContext callContext)
    {
        if (callContext.RequestHeaders == null)
        {
            throw new Exception("Request headers are null");
        }

        if (string.IsNullOrEmpty(PersistifyCredentials.AccessToken))
        {
            throw new Exception("Access token is null or empty");
        }

        var existingHeader = callContext.RequestHeaders.FirstOrDefault(h => h.Key == "Authorization");
        if (existingHeader != null)
        {
            callContext.RequestHeaders.Remove(existingHeader);
        }

        callContext.RequestHeaders.Add("Authorization", $"Bearer {PersistifyCredentials.AccessToken}");
    }
}
