using Persistify.Dtos.Request.Object;
using Persistify.Requests.Common;

namespace Persistify.Requests.StaticValidation.Objects;

public class IndexObjectStaticValidationRequest : StaticValidationRequest<IndexObjectRequestDto>
{
    public IndexObjectStaticValidationRequest(IndexObjectRequestDto dto) : base(dto)
    {
    }
}