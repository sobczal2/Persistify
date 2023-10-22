using System;
using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Persistify.Requests.Users;
using Persistify.Server.Domain.Users;
using Persistify.Server.Tests.Integration.Common;
using Persistify.TestHelpers.Assertions;
using ProtoBuf.Grpc;
using Xunit;

namespace Persistify.Server.Tests.Integration.Users;

public class SignInAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task SignInAsync_WhenCredentialsAreValid_ReturnsOk()
    {
        // Arrange
        var request = new SignInRequest { Username = RootCredentials.Username, Password = RootCredentials.Password };

        // Act
        var response = await UserService.SignInAsync(request, new CallContext());

        // Assert
        response.Should().NotBeNull();
        response.User.Username.Should().Be(RootCredentials.Username);
        response.User.Permission.Should().Be((int)Permission.All);
        response.AccessToken.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task SignInAsync_WhenUserDoesNotExist_ReturnsUnauthenticated()
    {
        // Arrange
        var request = new SignInRequest { Username = "invalid", Password = "invalid" };

        // Act
        var action = new Func<Task>(async () => await UserService.SignInAsync(request, new CallContext()));

        // Assert
        await action.Should().ThrowAsync<RpcException>().WithStatusCode(StatusCode.Unauthenticated);
    }

    [Fact]
    public async Task SignInAsync_WhenPasswordIsInvalid_ReturnsUnauthenticated()
    {
        // Arrange
        var request = new SignInRequest { Username = RootCredentials.Username, Password = "invalid" };

        // Act
        var action = new Func<Task>(async () => await UserService.SignInAsync(request, new CallContext()));

        // Assert
        await action.Should().ThrowAsync<RpcException>().WithStatusCode(StatusCode.Unauthenticated);
    }
}
