using System.Collections.Generic;
using Persistify.Pipeline.Contexts.Types;
using Persistify.Pipeline.Middlewares.Abstractions;
using Persistify.Pipeline.Middlewares.Common;
using Persistify.Pipeline.Middlewares.Types;
using Persistify.Pipeline.Orchestrators.Abstractions;
using Persistify.Pipeline.Wrappers.Abstractions;
using Persistify.Protos;

namespace Persistify.Pipeline.Orchestrators.Types;

public class CreateTypePipelineOrchestrator : PipelineOrchestratorBase<CreateTypePipelineContext, CreateTypeRequestProto
    , CreateTypeResponseProto>
{
    private readonly AddTypeToStoreMiddleware _addTypeToStoreMiddleware;

    private readonly
        RequestProtoValidationMiddleware<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>
        _protoValidationMiddleware;

    public CreateTypePipelineOrchestrator(
        IEnumerable<
                ICommonMiddlewareWrapper<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>>
            wrappers,
        RequestProtoValidationMiddleware<CreateTypePipelineContext, CreateTypeRequestProto
            , CreateTypeResponseProto> protoValidationMiddleware,
        AddTypeToStoreMiddleware addTypeToStoreMiddleware) : base(wrappers)
    {
        _protoValidationMiddleware = protoValidationMiddleware;
        _addTypeToStoreMiddleware = addTypeToStoreMiddleware;
    }

    protected override
        IEnumerable<IPipelineMiddleware<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>>
        CreatePipeline()
    {
        return new IPipelineMiddleware<CreateTypePipelineContext, CreateTypeRequestProto, CreateTypeResponseProto>[]
        {
            _protoValidationMiddleware,
            _addTypeToStoreMiddleware
        };
    }
}