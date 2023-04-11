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

public class SearchObjectsRequestHandler : CoreRequestHandler<SearchObjectsRequest, SearchObjectsResponseProto,
    SearchObjectsRequestProto, SearchObjectsRequestDto>
{
    public SearchObjectsRequestHandler(IMediator mediator,
        IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> validationErrorResponseProtoMapper,
        IProtoMapper<SearchObjectsRequestProto, SearchObjectsRequestDto> requestProtoMapper) : base(mediator,
        validationErrorResponseProtoMapper, requestProtoMapper)
    {
    }

    public override async ValueTask<SearchObjectsResponseProto> Handle(SearchObjectsRequest request,
        CancellationToken cancellationToken)
    {
        var requestDto = MapToDto(request.Proto);

        var validationResponse =
            await Mediator.Send(new SearchObjectsStaticValidationRequest(requestDto), cancellationToken);
        if (validationResponse.IsT1)
            return new SearchObjectsResponseProto
            {
                ValidationError = ValidationErrorResponseProtoMapper.MapToProto(validationResponse.AsT1)
            };

        return new SearchObjectsResponseProto
        {
            InternalError = new InternalErrorResponseProto
            {
                Message = "Not yet implemented"
            }
        };
    }
}