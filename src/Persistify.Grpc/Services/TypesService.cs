using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Grpc.Mappings;
using Persistify.Grpc.Protos;
using Persistify.Indexer.Core;
using Persistify.Indexer.Types;
using TypeDefinition = Persistify.Indexer.Types.TypeDefinition;
using TypeField = Persistify.Indexer.Types.TypeField;

namespace Persistify.Grpc.Services;

public class TypesService : Protos.TypesService.TypesServiceBase
{
    private readonly IPersistifyManager _persistifyManager;

    public TypesService(IPersistifyManager persistifyManager)
    {
        _persistifyManager = persistifyManager;
    }

    public override async Task<InitTypeResponse> InitType(
        InitTypeRequest request,
        ServerCallContext context
    )
    {
        var typeDefinition = new TypeDefinition(
            request.TypeDefinition.Name,
            request.TypeDefinition.TypeFields.MapToNormal()
        );
        if (await _persistifyManager.InitTypeAsync(typeDefinition))
        {
            return new InitTypeResponse();
        }

        context.Status = new Status(StatusCode.AlreadyExists, "Type already exists");
        return new InitTypeResponse();
    }

    public override async Task<ListTypesResponse> ListTypes(
        ListTypesRequest request,
        ServerCallContext context
    )
    {
        var types = await _persistifyManager.ListTypesAsync();

        return new ListTypesResponse
        {
            TypeDefinitions =
            {
                types.MapToProto()
            }
        };
    }

    public override async Task<DropTypeResponse> DropType(
        DropTypeRequest request,
        ServerCallContext context
    )
    {
        if (await _persistifyManager.DropTypeAsync(request.Name)) return new DropTypeResponse();
        context.Status = new Status(StatusCode.NotFound, "Type not found");
        return new DropTypeResponse();
    }
}