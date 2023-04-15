using System.Collections.Generic;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Types;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Wrappers.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Types;

public class ListTypesPipelineOrchestrator : PipelineOrchestratorBase<ListTypesPipelineContext, ListTypesRequestProto
    , ListTypesResponseProto>
{
    private readonly ListTypesFromStoreMiddleware _listTypesFromStoreMiddleware;

    private readonly
        RequestProtoValidationMiddleware<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>
        _protoValidationMiddleware;

    public ListTypesPipelineOrchestrator(
        IEnumerable<
                ICommonMiddlewareWrapper<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>>
            wrappers,
        RequestProtoValidationMiddleware<ListTypesPipelineContext, ListTypesRequestProto
            , ListTypesResponseProto> protoValidationMiddleware,
        ListTypesFromStoreMiddleware listTypesFromStoreMiddleware
    ) : base(wrappers)
    {
        _protoValidationMiddleware = protoValidationMiddleware;
        _listTypesFromStoreMiddleware = listTypesFromStoreMiddleware;
    }

    protected override
        IEnumerable<IPipelineMiddleware<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>>
        CreatePipeline()
    {
        return new IPipelineMiddleware<ListTypesPipelineContext, ListTypesRequestProto, ListTypesResponseProto>[]
        {
            _protoValidationMiddleware,
            _listTypesFromStoreMiddleware
        };
    }
}