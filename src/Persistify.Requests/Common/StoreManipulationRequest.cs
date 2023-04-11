using Mediator;
using OneOf;
using Persistify.Diagnostics.Attributes;
using Persistify.Diagnostics.Enums;
using Persistify.Stores.Common;

namespace Persistify.Requests.Common;

[PipelineStep(PipelineStepType.StoreManipulation)]
public abstract class StoreManipulationRequest<TDto, TStoreResult> : IRequest<OneOf<StoreSuccess<TStoreResult>, StoreError>>
{
    public StoreManipulationRequest(TDto dto)
    {
        Dto = dto;
    }

    public TDto Dto { get; }
}