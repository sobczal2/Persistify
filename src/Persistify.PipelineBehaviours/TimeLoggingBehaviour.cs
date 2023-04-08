using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Microsoft.Extensions.Logging;
using Persistify.Diagnostics;

namespace Persistify.PipelineBehaviours;

public class TimeLoggingBehaviour<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    private readonly ILogger<TimeLoggingBehaviour<TMessage, TResponse>> _logger;

    public TimeLoggingBehaviour(ILogger<TimeLoggingBehaviour<TMessage, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(TMessage message, CancellationToken cancellationToken,
        MessageHandlerDelegate<TMessage, TResponse> next)
    {
        var stopwatch = Stopwatch.StartNew();

        var response = await next(message, cancellationToken);

        stopwatch.Stop();

        var type = message.GetType();
        _logger.LogInformation(
            "Step {TMessage}({StepType}) handled in {ElapsedMicroseconds}us",
            type.Name,
            type.GetPipelineStepType(),
            stopwatch.Elapsed.Microseconds);

        return response;
    }
}