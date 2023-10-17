using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Persistify.Requests.Users;
using Persistify.Server.Tests.Integration.Common;
using Xunit;

namespace Persistify.Server.Tests.Integration.Users;

public class DeleteUserAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task DeleteUserAsync_WhenUserExists_ReturnsOkAndDeletesUser()
    {
        // Arrange
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var username = "test";
        var password = "test";
        await CreateUserAsync(username, password);
        var request = new DeleteUserRequest { Username = username };

        // Act
        var response = await UserService.DeleteUserAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
        try
        {
            var getUserResponse =
                await UserService.GetUserAsync(new GetUserRequest { Username = request.Username }, callContext);
            getUserResponse.Should().BeNull();
        }
        catch (RpcException ex)
        {
            ex.StatusCode.Should().Be(StatusCode.NotFound);
        }
    }
}
