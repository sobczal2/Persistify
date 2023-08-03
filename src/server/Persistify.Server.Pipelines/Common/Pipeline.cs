using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Persistify.Helpers.ErrorHandling;
using Persistify.Server.Management.Abstractions.Domain;
using Persistify.Server.Management.Domain;
using Persistify.Server.Management.Domain.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.Pipelines.Common;

public abstract class Pipeline<TContext, TRequest, TResponse>
    where TContext : class, IPipelineContext<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
    private readonly ILogger<Pipeline<TContext, TRequest, TResponse>> _logger;
    private readonly ITransactionManager _transactionManager;

    protected Pipeline(
        ILogger<Pipeline<TContext, TRequest, TResponse>> logger,
        ITransactionManager transactionManager
    )

    {
        _logger = logger;
        _transactionManager = transactionManager;
    }

    protected abstract IEnumerable<PipelineStage<TContext, TRequest, TResponse>> PipelineStages { get; }

    protected abstract TContext CreateContext(TRequest request);

    protected abstract TResponse CreateResponse(TContext context);
    protected abstract (bool write, bool global, IEnumerable<int> templateIds) GetTransactionInfo(TContext context);

    public async ValueTask<TResponse> ProcessAsync(TRequest request)
    {
        if (request is null)
        {
            throw new ValidationException(typeof(TRequest).Name, "Request cannot be null");
        }

        var context = CreateContext(request);

        TransactionState.Current = new Transaction();
        var (write, global, templateIds) = GetTransactionInfo(context);
        await _transactionManager.BeginAsync(templateIds, write, global);

        try
        {
            foreach (var pipelineStage in PipelineStages)
            {
                _logger.LogInformation("Processing pipeline stage {StageName}", pipelineStage.Name);

                await pipelineStage.ProcessAsync(context);
            }

            await _transactionManager.CommitAsync();
        }
        catch (Exception ex)
        {
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
