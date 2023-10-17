using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Requests.Users;
using Persistify.Server.Tests.Integration.Common;
using ProtoBuf.Grpc;
using Xunit;

namespace Persistify.Server.Tests.Integration.Users;

public class ChangeUserPasswordAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task ChangeUserPasswordAsync_WhenUserExists_ReturnsOkAndChangesPassword()
    {
        // Arrange
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var username = "test";
        var password = "test";
        await CreateUserAsync(username, password);
        var request = new ChangeUserPasswordRequest { Username = username, Password = "test2" };

        // Act
        var response = await UserService.ChangeUserPasswordAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
        var signInResponse =
            await UserService.SignInAsync(new SignInRequest { Username = username, Password = "test2" },
                new CallContext());
        signInResponse.Should().NotBeNull();
    }
}
