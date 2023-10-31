using Grpc.Core;
using Grpc.Net.Client;
using Persistify.Client.LowLevel.Documents;
using Persistify.Client.LowLevel.Exceptions;
using Persistify.Client.LowLevel.PresetAnalyzers;
using Persistify.Client.LowLevel.Templates;
using Persistify.Client.LowLevel.Users;
using Persistify.Helpers.Results;
using Persistify.Requests.Users;
using ProtoBuf.Grpc;

namespace Persistify.Client.LowLevel.Core;

public class PersistifyLowLevelClient : IPersistifyLowLevelClient, IDisposable
{
    internal PersistifyLowLevelClient(
        GrpcChannel channel,
        PersistifyCredentials persistifyCredentials
    )
    {
        Channel = channel;
        PersistifyCredentials = persistifyCredentials;
        Users = new UsersClient(this);
        Templates = new TemplatesClient(this);
        Documents = new DocumentsClient(this);
        PresetAnalyzers = new PresetAnalyzersClient(this);
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
    public IPresetAnalyzersClient PresetAnalyzers { get; }

    internal async Task<Result<TResponse>> CallAuthenticatedServiceAsync<TResponse>(
        Func<CallContext, Task<Result<TResponse>>> serviceCall,
        CallContext? callContext = null
    )
    {
        callContext ??= new CallContext(new CallOptions(new Metadata()));

        if (callContext.Value.RequestHeaders == null)
        {
            return new Result<TResponse>(
                new PersistifyLowLevelClientException("Request headers are null")
            );
        }

        if (string.IsNullOrEmpty(PersistifyCredentials.AccessToken))
        {
            var authenticateResult = await AuthenticateAsync(callContext);
            if (authenticateResult.Failure)
            {
                return new Result<TResponse>(authenticateResult.Exception);
            }
        }

        SetAuthorizationHeader(callContext.Value);

        var serviceCallResult = await serviceCall(callContext.Value);

        if (
            serviceCallResult
            is not
            {
                Failure: true,
                Exception: RpcException { StatusCode: StatusCode.Unauthenticated }
            }
        )
        {
            return serviceCallResult;
        }

        var handleUnauthenticated = await SaveTokenAsync(callContext);
        if (handleUnauthenticated.Failure)
        {
            return new Result<TResponse>(handleUnauthenticated.Exception);
        }

        SetAuthorizationHeader(callContext.Value);
        return await serviceCall(callContext.Value);
    }

    private async Task<Result> AuthenticateAsync(CallContext? callContext)
    {
        var result = await this.SignInAsync(
            new SignInRequest { Username = PersistifyCredentials.Username, Password = PersistifyCredentials.Password },
            callContext
        );

        return result.Match(
            res =>
            {
                PersistifyCredentials.AccessToken = res.AccessToken;
                PersistifyCredentials.RefreshToken = res.RefreshToken;
                return Result.Ok;
            },
            ex =>
            {
                PersistifyCredentials.AccessToken = null;
                PersistifyCredentials.RefreshToken = null;
                return new Result(ex);
            }
        );
    }

    private async Task<Result> SaveTokenAsync(CallContext? callContext)
    {
        if (string.IsNullOrEmpty(PersistifyCredentials.RefreshToken))
        {
            throw new Exception("Refresh token is null or empty");
        }

        var refreshTokenResult = await this.RefreshTokenAsync(
            new RefreshTokenRequest { RefreshToken = PersistifyCredentials.RefreshToken },
            callContext
        );

        if (refreshTokenResult.Success)
        {
            PersistifyCredentials.AccessToken = refreshTokenResult.Value.AccessToken;
            PersistifyCredentials.RefreshToken = refreshTokenResult.Value.RefreshToken;
            return Result.Ok;
        }

        if (
            refreshTokenResult is { Failure: true, Exception: RpcException ex }
            && ex.StatusCode != StatusCode.Unauthenticated
        )
        {
            return new Result(ex);
        }

        var authenticateResult = await AuthenticateAsync(callContext);

        return authenticateResult.Match(() => Result.Ok, exception => new Result(exception));
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

        var existingHeader = callContext.RequestHeaders.FirstOrDefault(
            h => h.Key == "Authorization"
        );
        if (existingHeader != null)
        {
            callContext.RequestHeaders.Remove(existingHeader);
        }

        callContext.RequestHeaders.Add(
            "Authorization",
            $"Bearer {PersistifyCredentials.AccessToken}"
        );
    }
}
