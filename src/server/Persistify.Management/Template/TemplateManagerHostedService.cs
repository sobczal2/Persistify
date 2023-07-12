using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistify.Server.Configuration.Settings;

namespace Persistify.Management.Template;

public class TemplateManagerHostedService : IHostedService, IDisposable
{
    private readonly HostedServicesSettings _hostedServicesSettings;
    private readonly ILogger<TemplateManagerHostedService> _logger;
    private readonly ITemplateManager _templateManager;
    private Timer? _timer;

    public TemplateManagerHostedService(
        ITemplateManager templateManager,
        ILogger<TemplateManagerHostedService> logger,
        IOptions<HostedServicesSettings> hostedServicesSettings
    )
    {
        _templateManager = templateManager;
        _logger = logger;
        _hostedServicesSettings = hostedServicesSettings.Value;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Loading saved templates");
        await _templateManager.LoadAsync();

        _logger.LogInformation("Starting template manager hosted service");

        _timer = new Timer(
            // ReSharper disable once AsyncVoidLambda
            async _ =>
            {
                _logger.LogInformation("Saving templates");
                await _templateManager.SaveAsync();
            },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(_hostedServicesSettings.TemplateManagerIntervalSeconds)
        );
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving templates");
        await _templateManager.SaveAsync();

        _logger.LogInformation("Stopping template manager hosted service");

        _timer?.Change(Timeout.Infinite, 0);
    }
}
