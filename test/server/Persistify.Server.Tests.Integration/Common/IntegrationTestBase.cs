using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;
using Persistify.Services;
using ProtoBuf.Grpc.Client;
using Xunit;

namespace Persistify.Server.Tests.Integration.Common;

public class IntegrationTestBase : IClassFixture<PersistifyServerWebApplicationFactory>
{
    private readonly PersistifyServerWebApplicationFactory _factory;
    private readonly GrpcChannel _grpcChannel;

    public IntegrationTestBase(PersistifyServerWebApplicationFactory factory)
    {
        _factory = factory;
        var rootSettings = _factory.Services.GetRequiredService<IOptions<RootSettings>>().Value;
        RootCredentials = (
            rootSettings.Username,
            rootSettings.Password
        );
        var client = _factory.CreateDefaultClient();
        _grpcChannel = GrpcChannel.ForAddress(_factory.Server.BaseAddress, new GrpcChannelOptions
        {
            HttpClient = client
        });
    }

    public IUserService UserService => _grpcChannel.CreateGrpcService<IUserService>();
    public ITemplateService TemplateService => _grpcChannel.CreateGrpcService<ITemplateService>();
    public IDocumentService DocumentService => _grpcChannel.CreateGrpcService<IDocumentService>();

    public (string Username, string Password) RootCredentials { get; }
}
