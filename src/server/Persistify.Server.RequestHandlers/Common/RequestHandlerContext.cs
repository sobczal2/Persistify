using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistify.Requests.Common;
using Persistify.Responses.Common;
using Persistify.Server.ErrorHandling.ExceptionHandlers;
using Persistify.Server.Management.Transactions;
using Persistify.Server.Validation.Common;

namespace Persistify.Server.CommandHandlers.Common;

public class RequestHandlerContext<TRequest, TResponse> : IRequestHandlerContext<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResponse
{
    private readonly IExceptionHandler _exceptionHandler;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IValidator<TRequest> _validator;

    public RequestHandlerContext(
        IValidator<TRequest> validator,
        ILoggerFactory loggerFactory,
        ITransactionState transactionState,
        IExceptionHandler exceptionHandler
    )
    {
        _validator = validator;
        _loggerFactory = loggerFactory;
        TransactionState = transactionState;
        _exceptionHandler = exceptionHandler;
    }

    public ITransactionState TransactionState { get; }

    public void HandleException(Exception exception)
    {
        _exceptionHandler.Handle(exception);
    }

    public ITransaction CurrentTransaction => TransactionState.GetCurrentTransaction();

    public async ValueTask ValidateAsync(TRequest request)
    {
        (await _validator.ValidateAsync(request)).OnFailure(ex => throw ex);
    }

    public ILogger<T> CreateLogger<T>()
    {
        return _loggerFactory.CreateLogger<T>();
    }
}
