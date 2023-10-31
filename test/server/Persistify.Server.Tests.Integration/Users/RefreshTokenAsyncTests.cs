using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Requests.Users;
using Persistify.Server.Tests.Integration.Common;
using ProtoBuf.Grpc;
using Xunit;

namespace Persistify.Server.Tests.Integration.Users;

public class RefreshTokenAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task RefreshTokenAsync_WhenRefreshTokenIsValid_ReturnsOk()
    {
        // Arrange
        var signInRequest = new SignInRequest
        {
            Username = RootCredentials.Username, Password = RootCredentials.Password
        };
        var signInResponse = await UserService.SignInAsync(signInRequest, new CallContext());
        var request = new RefreshTokenRequest
        {
            Username = RootCredentials.Username, RefreshToken = signInResponse.RefreshToken
        };

        // Act
        var response = await UserService.RefreshTokenAsync(request, new CallContext());

        // Assert
        response.Should().NotBeNull();
        response.AccessToken.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBeNullOrEmpty();
    }
}
