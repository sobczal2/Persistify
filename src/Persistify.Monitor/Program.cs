using Microsoft.EntityFrameworkCore;
using Persistify.Client;
using Persistify.Monitor.Database;
using Persistify.Monitor.HostedServices;
using Persistify.Monitor.Hubs;
using Persistify.Monitor.Services;
using Serilog;

Log.Logger = new LoggerConfiguration().MinimumLevel
    .Information()
    // .MinimumLevel.Error()
    .WriteTo.Async(c => { c.Console(); })
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddSignalR();

    builder.Services.AddHostedService<PipelineStreamHostedService>();
    builder.Services.AddPersistifyClient(opt => { opt.BaseAddress = "http://localhost:5001"; });

    builder.Services.AddDbContext<MonitorDbContext>(opt => { opt.UseSqlite("Data Source=monitor.db"); });

    builder.Services.AddSingleton<StatusProvider>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.MapBlazorHub();
    app.MapHub<MonitorHub>("/monitoring-hub");
    app.MapFallbackToPage("/_Host");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}