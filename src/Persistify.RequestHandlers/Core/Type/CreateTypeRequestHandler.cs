using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Persistify.Dtos.Request.Type;
using Persistify.Dtos.Response.Shared;
using Persistify.ProtoMappers;
using Persistify.Protos;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.Core.Type;
using Persistify.Requests.StaticValidation.Type;
using Persistify.Requests.StoreManipulation.Types;
using Persistify.Stores.Common;

namespace Persistify.RequestHandlers.Core.Type;

public class CreateTypeRequestHandler : CoreRequestHandler<CreateTypeRequest, CreateTypeResponseProto,
    CreateTypeRequestProto, CreateTypeRequestDto>
{
    public CreateTypeRequestHandler(IMediator mediator,
        IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> validationErrorResponseProtoMapper,
        IProtoMapper<CreateTypeRequestProto, CreateTypeRequestDto> requestProtoMapper) : base(mediator,
        validationErrorResponseProtoMapper, requestProtoMapper)
    {
    }

    public override async ValueTask<CreateTypeResponseProto> Handle(CreateTypeRequest request,
        CancellationToken cancellationToken)
    {
        var requestDto = MapToDto(request.Proto);

        var validationResponse =
            await Mediator.Send(new CreateTypeStaticValidationRequest(requestDto), cancellationToken);

        if (validationResponse.IsT1)
            return new CreateTypeResponseProto
            {
                ValidationError = ValidationErrorResponseProtoMapper.MapToProto(validationResponse.AsT1)
            };

        var storeResponse =
            await Mediator.Send(new CreateTypeInStoreRequest(requestDto.TypeDefinition), cancellationToken);

        return storeResponse.Match(
            _ => new CreateTypeResponseProto()
            {
                Success = new CreateTypeSuccessResponseProto()
            },
            error =>
            {
                if (error.Type == StoreErrorType.AlreadyExists)
                    return new CreateTypeResponseProto()
                    {
                        ValidationError = new ValidationErrorResponseProto()
                        {
                            Errors =
                            {
                                new ValidationErrorProto()
                                {
                                    Field = "TypeName",
                                    Message = "Type already exists"
                                }
                            }
                        }
                    };
                return new CreateTypeResponseProto()
                {
                    InternalError = new InternalErrorResponseProto()
                    {
                        Message = error.Message
                    }
                };
            }
        );
    }
}