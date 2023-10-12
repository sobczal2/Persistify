using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Specialized;
using Grpc.Core;

namespace Persistify.TestHelpers.Assertions;

public static class RpcExceptionAssertions
{
    public static async Task<ExceptionAssertions<TException>> WithStatusCode<TException>(
        this Task<ExceptionAssertions<TException>> task,
        StatusCode expectedStatusCode
    )
        where TException : RpcException
    {
        var exceptionAssertions = await task;
        exceptionAssertions.Which.StatusCode.Should().Be(expectedStatusCode);
        return exceptionAssertions;
    }
}
