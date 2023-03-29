using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Persistify.Grpc.Protos;
using Persistify.Indexer.Types;
using TypeDefinition = Persistify.Indexer.Types.TypeDefinition;
using TypeField = Persistify.Indexer.Types.TypeField;

namespace Persistify.Grpc.Services;

public class TypesService : Protos.TypesService.TypesServiceBase
{
    private readonly ITypeStore _typeStore;

    public TypesService(ITypeStore typeStore)
    {
        _typeStore = typeStore;
    }

    public override async Task<InitTypeResponse> InitType(
        InitTypeRequest request,
        ServerCallContext context
    )
    {
        var typeDefinition = new TypeDefinition(
            request.TypeDefinition.Name,
            request.TypeDefinition.TypeFields
                .Select(x => new TypeField(x.Path, x.Indexed))
                .ToArray()
        );
        if (await _typeStore.InitTypeAsync(typeDefinition)) return new InitTypeResponse();
        context.Status = new Status(StatusCode.AlreadyExists, "Type already exists");
        return new InitTypeResponse();
    }

    public override async Task<ListTypesResponse> ListTypes(
        ListTypesRequest request,
        ServerCallContext context
    )
    {
        var types = await _typeStore.ListTypesAsync();
        return new ListTypesResponse
        {
            TypeDefinitions =
            {
                types.Select(
                    x =>
                        new Protos.TypeDefinition
                        {
                            Name = x.Name,
                            TypeFields =
                            {
                                x.TypeFields.Select(
                                    y => new Protos.TypeField { Path = y.Path, Indexed = y.Indexed }
                                )
                            }
                        }
                )
            }
        };
    }

    public override async Task<DropTypeResponse> DropType(
        DropTypeRequest request,
        ServerCallContext context
    )
    {
        if (await _typeStore.DropTypeAsync(request.Name)) return new DropTypeResponse();
        context.Status = new Status(StatusCode.NotFound, "Type not found");
        return new DropTypeResponse();
    }
}