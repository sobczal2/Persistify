using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using Persistify.Dtos.Request.Object;
using Persistify.Dtos.Response.Shared;
using Persistify.ProtoMappers;
using Persistify.Protos;
using Persistify.RequestHandlers.Common;
using Persistify.Requests.Core.Objects;
using Persistify.Requests.StaticValidation.Objects;

namespace Persistify.RequestHandlers.Core.Objects;

public class IndexObjectRequestHandler : CoreRequestHandler<IndexObjectRequest, IndexObjectResponseProto,
    IndexObjectRequestProto, IndexObjectRequestDto>
{
    public IndexObjectRequestHandler(IMediator mediator,
        IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> validationErrorResponseProtoMapper,
        IProtoMapper<IndexObjectRequestProto, IndexObjectRequestDto> requestProtoMapper) : base(mediator,
        validationErrorResponseProtoMapper, requestProtoMapper)
    {
    }
    
    public override async ValueTask<IndexObjectResponseProto> Handle(IndexObjectRequest request,
        CancellationToken cancellationToken)
    {
        var requestDto = MapToDto(request.Proto);

        var validationResponse =
            await Mediator.Send(new IndexObjectStaticValidationRequest(requestDto), cancellationToken);
        if (validationResponse.IsT1)
            return new IndexObjectResponseProto
            {
                ValidationError = ValidationErrorResponseProtoMapper.MapToProto(validationResponse.AsT1)
            };

        return new IndexObjectResponseProto
        {
            InternalError = new InternalErrorResponseProto
            {
                Message = "Not yet implemented"
            }
        };
    }
}