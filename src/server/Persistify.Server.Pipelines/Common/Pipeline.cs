using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Domain;
using Persistify.Server.Persistence.Core.Transactions;
using Persistify.Server.Persistence.Core.Transactions.TLogs;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Common;

public abstract class Pipeline<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<Pipeline<TContext, TRequest, TResponse>> _logger;
    private readonly ITransactionManager _transactionManager;
    private readonly ISystemClock _systemClock;

    protected Pipeline(
        ILogger<Pipeline<TContext, TRequest, TResponse>> logger,
        ITransactionManager transactionManager,
        ISystemClock systemClock
    )

    {
        _logger = logger;
        _transactionManager = transactionManager;
        _systemClock = systemClock;
    }

    protected abstract IEnumerable<PipelineStage<TContext, TRequest, TResponse>> PipelineStages { get; }

    protected abstract TContext CreateContext(TRequest request);

    protected abstract TResponse CreateResponse(TContext context);
    protected abstract Transaction CreateTransaction(TContext context);

    public async ValueTask<TResponse> ProcessAsync(TRequest request)
    {
        if (request is null)
        {
            throw new ValidationException(typeof(TRequest).Name, "Request cannot be null");
        }

        var context = CreateContext(request);

        TransactionState.Current = CreateTransaction(context);
        await _transactionManager.BeginAsync();

        string pipelineStageName = "Unknown";
        try
        {
            foreach (var pipelineStage in PipelineStages)
            {
                pipelineStageName = pipelineStage.Name;
                TransactionState.RequiredCurrent.TransactionLogs.Add(
                    new PipelineStageLog(
                        pipelineStageName,
                        PipelineStageState.Started,
                        _systemClock.UtcNow.UtcDateTime
                    )
                );

                await pipelineStage.ProcessAsync(context);

                TransactionState.RequiredCurrent.TransactionLogs.Add(
                    new PipelineStageLog(
                        pipelineStageName,
                        PipelineStageState.Completed,
                        _systemClock.UtcNow.UtcDateTime
                    )
                );
            }

            await _transactionManager.CommitAsync();
        }
        catch (Exception ex)
        {
            TransactionState.RequiredCurrent.TransactionLogs.Add(
                new PipelineStageLog(
                    pipelineStageName,
                    PipelineStageState.Failed,
                    _systemClock.UtcNow.UtcDateTime
                )
            );
            await _transactionManager.RollbackAsync();

            if (ex is ExceptionWithStatus exceptionWithStatus)
            {
                throw new RpcException(exceptionWithStatus.Status);
            }

            _logger.LogError(ex, "Error while processing pipeline");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
        finally
        {
            TransactionState.Current = null;
        }

        return CreateResponse(context);
    }
}
