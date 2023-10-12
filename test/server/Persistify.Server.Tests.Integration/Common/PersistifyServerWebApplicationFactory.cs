using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Persistify.Server.Tests.Integration.Common;

public class PersistifyServerWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string DataPath = "persistify_test_data";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (!Directory.Exists(DataPath))
            Directory.CreateDirectory(DataPath);
        builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
        if (Directory.Exists(DataPath))
            Directory.Delete(DataPath, true);
        base.Dispose(disposing);
    }
}
