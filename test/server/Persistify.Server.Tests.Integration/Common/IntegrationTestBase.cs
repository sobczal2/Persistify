using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistify.Requests.Users;
using Persistify.Server.Configuration.Settings;
using Persistify.Services;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

namespace Persistify.Server.Tests.Integration.Common;

public class IntegrationTestBase : IDisposable
{
    private readonly PersistifyServerWebApplicationFactory _factory;
    private readonly GrpcChannel _grpcChannel;

    public IntegrationTestBase()
    {
        _factory = new PersistifyServerWebApplicationFactory();
        var rootSettings = _factory.Services.GetRequiredService<IOptions<RootSettings>>().Value;
        RootCredentials = (rootSettings.Username, rootSettings.Password);
        var client = _factory.CreateDefaultClient();
        _grpcChannel = GrpcChannel.ForAddress(
            _factory.Server.BaseAddress,
            new GrpcChannelOptions { HttpClient = client }
        );
    }

    public IUserService UserService => _grpcChannel.CreateGrpcService<IUserService>();
    public ITemplateService TemplateService => _grpcChannel.CreateGrpcService<ITemplateService>();
    public IDocumentService DocumentService => _grpcChannel.CreateGrpcService<IDocumentService>();

    public (string Username, string Password) RootCredentials { get; }

    public void Dispose()
    {
        _factory.Dispose();
        _grpcChannel.Dispose();
    }

    public async Task<CallContext> GetAuthorizedCallContextAsync(string username, string password)
    {
        var request = new SignInRequest { Username = username, Password = password };
        var response = await UserService.SignInAsync(request, new CallContext());
        return new CallContext(
            new CallOptions(new Metadata { new("Authorization", $"Bearer {response.AccessToken}") })
        );
    }

    protected async Task<CallContext> GetAuthorizedCallContextAsRootAsync()
    {
        return await GetAuthorizedCallContextAsync(
            RootCredentials.Username,
            RootCredentials.Password
        );
    }

    protected async Task CreateUserAsync(string username, string password)
    {
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var request = new CreateUserRequest { Username = username, Password = password };
        await UserService.CreateUserAsync(request, callContext);
    }
}
