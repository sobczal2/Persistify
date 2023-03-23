using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Grpc.Protos;
using Persistify.Indexer;
using TypeDefinition = Persistify.Indexer.Types.TypeDefinition;

namespace Persistify.Grpc.Services;

public class TypesService : Protos.Types.TypesBase
{
    private readonly ITypeManager _typeManager;

    public TypesService(ITypeManager typeManager)
    {
        _typeManager = typeManager;
    }

    public override Task<InitTypeResponse> InitType(InitTypeRequest request, ServerCallContext context)
    {
        var typeDefinition = new TypeDefinition(request.TypeDefinition.Name,
            request.TypeDefinition.TypeFields.Select(x => new Indexer.Types.TypeField(x.Path, x.Indexed)).ToArray());
        _typeManager.InitTypeAsync(typeDefinition);
        return Task.FromResult(new InitTypeResponse()
        {
            Metadata = new ResponseMetadata()
            {
                Message = "Type initialized",
                ResponseStatus = ResponseStatus.Ok
            }
        });
    }

    public override Task<ListTypesResponse> ListTypes(ListTypesRequest request, ServerCallContext context)
    {
        return base.ListTypes(request, context);
    }

    public override Task<DropTypeResponse> DropType(DropTypeRequest request, ServerCallContext context)
    {
        return base.DropType(request, context);
    }
}