using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Persistify.Server.Tests.Integration.Common;

public class PersistifyServerWebApplicationFactory : WebApplicationFactory<Program>
{
    private DirectoryInfo? _dataDirectoryInfo;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _dataDirectoryInfo = Directory.CreateTempSubdirectory();
        builder.UseConfiguration(new ConfigurationBuilder()
            .AddJsonFile("appsettings.Testing.json")
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Storage:DataPath", _dataDirectoryInfo.FullName }
            }!)
            .Build()
        );

        builder.ConfigureLogging(cfg =>
        {
            cfg.ClearProviders();
        });
    }

    public override async ValueTask DisposeAsync()
    {
        _dataDirectoryInfo?.Delete(true);
        await base.DisposeAsync();
    }
}
