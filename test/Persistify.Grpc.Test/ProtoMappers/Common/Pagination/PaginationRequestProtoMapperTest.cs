using System;
using Persistify.Dtos.Common.Pagination;
using Persistify.ProtoMappers.Common.Pagination;
using Persistify.Protos;
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