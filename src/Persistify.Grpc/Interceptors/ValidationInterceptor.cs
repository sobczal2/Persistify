using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Persistify.Validators.Common;

namespace Persistify.Grpc.Interceptors;

public class ValidationInterceptor : Interceptor
{
    private readonly IValidatorFactory _validatorFactory;

    public ValidationInterceptor(IValidatorFactory validatorFactory)
    {
        _validatorFactory = validatorFactory;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation
    )
    {
        var validator = _validatorFactory.GetValidator<TRequest>();
        if (validator == null)
            return base.UnaryServerHandler(request, context, continuation);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    validationResult.Message ?? "Invalid argument"
                )
            );

        return base.UnaryServerHandler(request, context, continuation);
    }
}