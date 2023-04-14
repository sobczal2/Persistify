using System.Linq;
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
using Persistify.Requests.StoreManipulation.Documents;
using Persistify.Requests.StoreManipulation.Indexes;
using Persistify.Requests.StoreManipulation.Types;
using Persistify.Tokenizer;

namespace Persistify.RequestHandlers.Core.Objects;

public class IndexObjectRequestHandler : CoreRequestHandler<IndexObjectRequest, IndexObjectResponseProto,
    IndexObjectRequestProto, IndexObjectRequestDto>
{
    private readonly IProtoMapper<ValidationErrorProto, ValidationErrorDto> _validationErrorProtoMapper;
    private readonly ITokenizer _tokenizer;

    public IndexObjectRequestHandler(IMediator mediator,
        IProtoMapper<ValidationErrorResponseProto, ValidationErrorResponseDto> validationErrorResponseProtoMapper,
        IProtoMapper<IndexObjectRequestProto, IndexObjectRequestDto> requestProtoMapper,
        IProtoMapper<ValidationErrorProto, ValidationErrorDto> validationErrorProtoMapper,
        ITokenizer tokenizer) : base(mediator,
        validationErrorResponseProtoMapper, requestProtoMapper)
    {
        _validationErrorProtoMapper = validationErrorProtoMapper;
        _tokenizer = tokenizer;
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
        
        var typeExistsResponse = await Mediator.Send(new TypeExistsInStoreRequest(requestDto.TypeName), cancellationToken);
        if (typeExistsResponse.IsT0)
        {
            if (!typeExistsResponse.AsT0.Data)
                return new IndexObjectResponseProto
                {
                    ValidationError = new ValidationErrorResponseProto
                    {
                        Errors =
                        {
                            new ValidationErrorProto
                            {
                                Field = "TypeName",
                                Message = "Type does not exist"
                            }
                        }
                    }
                };
        }
        else
        {
            return new IndexObjectResponseProto
            {
                InternalError = new InternalErrorResponseProto()
                {
                    Message = "An error occurred while checking if type exists"
                }
            };
        }
        
        var typeDefinitionResponse = await Mediator.Send(new GetTypeFromStoreRequest(requestDto.TypeName), cancellationToken);
        if (typeDefinitionResponse.IsT1)
        {
            return new IndexObjectResponseProto
            {
                InternalError = new InternalErrorResponseProto()
                {
                    Message = "An error occurred while getting type definition"
                }
            };
        }
        
        var typeDefinition = typeDefinitionResponse.AsT0.Data;
        var typeDefinitionValidationErrors = typeDefinition.Validate(requestDto.Data);
        
        if (typeDefinitionValidationErrors.Any())
        {
            return new IndexObjectResponseProto
            {
                ValidationError = new ValidationErrorResponseProto
                {
                    Errors =
                    {
                        _validationErrorProtoMapper.MapToProtoRF(typeDefinitionValidationErrors)
                    }
                }
            };
        }
        
        var createDocumentResponse = await Mediator.Send(new CreateDocumentInStoreRequest(requestDto.Data), cancellationToken);
        if (createDocumentResponse.IsT1)
        {
            return new IndexObjectResponseProto
            {
                InternalError = new InternalErrorResponseProto()
                {
                    Message = "An error occurred while creating document"
                }
            };
        }
        
        var documentId = createDocumentResponse.AsT0.Data;
        
        var tokens = typeDefinition.TokenizeStringFields(requestDto.Data, _tokenizer);
        
        var addTokensResponse = await Mediator.Send(new AddTokensInStoreRequest((typeDefinition.TypeName, tokens, documentId)), cancellationToken);
        
        if (addTokensResponse.IsT1)
        {
            // TODO: Delete document
            return new IndexObjectResponseProto
            {
                InternalError = new InternalErrorResponseProto()
                {
                    Message = "An error occurred while adding tokens"
                }
            };
        }
        
        return new IndexObjectResponseProto
        {
            Success = new IndexObjectSuccessResponseProto()
            {
                Id = documentId
            }
        };
    }
}