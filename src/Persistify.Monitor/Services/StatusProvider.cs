namespace Persistify.Monitor.Services;

public class StatusProvider
{
    private readonly ILogger<StatusProvider> _logger;

    public bool PipelineStreamConnected
    {
        get
        {
            return _pipelineStreamConnected;
        }
        set
        {
            if(_pipelineStreamConnected == value) return;
            _logger.LogInformation($"PipelineStreamConnected: {value}");
            _pipelineStreamConnected = value;
        }
    }
    
    private bool _pipelineStreamConnected;

    public StatusProvider(ILogger<StatusProvider> logger)
    {
        _logger = logger;
        PipelineStreamConnected = false;
    }
}