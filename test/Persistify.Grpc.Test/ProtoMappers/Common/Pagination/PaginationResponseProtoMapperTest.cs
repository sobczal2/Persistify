using System;
using Persistify.ExternalDtos.Common.Pagination;
using Persistify.Grpc.ProtoMappers.Common.Pagination;
using Xunit;

namespace Persistify.Grpc.Test.ProtoMappers.Common.Pagination;

public class PaginationResponseProtoMapperTest : ProtoMapperTestBase<PaginationResponseProtoMapper,
    PaginationResponseProto, PaginationResponseDto>
{
    protected override PaginationResponseDto CreateDto()
    {
        return new PaginationResponseDto
        {
            PageNumber = Random.Shared.Next(1, 100),
            PageSize = Random.Shared.Next(1, 100),
            TotalItems = Random.Shared.Next(1, 100),
            TotalPages = Random.Shared.Next(1, 100)
        };
    }

    protected override PaginationResponseProto CreateProto()
    {
        return new PaginationResponseProto
        {
            PageNumber = Random.Shared.Next(1, 100),
            PageSize = Random.Shared.Next(1, 100),
            TotalItems = Random.Shared.Next(1, 100),
            TotalPages = Random.Shared.Next(1, 100)
        };
    }

    protected override void AssertAreEqual(PaginationResponseDto dto, PaginationResponseProto proto)
    {
        Assert.Equal(dto.PageNumber, proto.PageNumber);
        Assert.Equal(dto.PageSize, proto.PageSize);
        Assert.Equal(dto.TotalItems, proto.TotalItems);
        Assert.Equal(dto.TotalPages, proto.TotalPages);
    }
}