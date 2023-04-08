using System;
using Persistify.ExternalDtos.Common.Pagination;
using Persistify.Grpc.ProtoMappers.Common.Pagination;
using Xunit;

namespace Persistify.Grpc.Test.ProtoMappers.Common.Pagination;

public class PaginationRequestProtoMapperTest : ProtoMapperTestBase<PaginationRequestProtoMapper, PaginationRequestProto
    , PaginationRequestDto>
{
    protected override PaginationRequestDto CreateDto()
    {
        return new PaginationRequestDto
        {
            PageNumber = Random.Shared.Next(1, 100),
            PageSize = Random.Shared.Next(1, 100)
        };
    }

    protected override PaginationRequestProto CreateProto()
    {
        return new PaginationRequestProto
        {
            PageNumber = Random.Shared.Next(1, 100),
            PageSize = Random.Shared.Next(1, 100)
        };
    }

    protected override void AssertAreEqual(PaginationRequestDto dto, PaginationRequestProto proto)
    {
        Assert.Equal(dto.PageNumber, proto.PageNumber);
        Assert.Equal(dto.PageSize, proto.PageSize);
    }
}