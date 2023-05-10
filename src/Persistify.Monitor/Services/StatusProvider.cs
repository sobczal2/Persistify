namespace Persistify.Monitor.Services;

public class StatusProvider
{
    private readonly ILogger<StatusProvider> _logger;

    private bool _pipelineStreamConnected;

    public StatusProvider(ILogger<StatusProvider> logger)
    {
        _logger = logger;
        PipelineStreamConnected = false;
    }

    public bool PipelineStreamConnected
    {
        get => _pipelineStreamConnected;
        set
        {
            if (_pipelineStreamConnected == value) return;
            _logger.LogInformation($"PipelineStreamConnected: {value}");
            _pipelineStreamConnected = value;
        }
    }
}