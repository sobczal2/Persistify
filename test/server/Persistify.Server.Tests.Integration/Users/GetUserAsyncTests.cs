using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Requests.Users;
using Persistify.Server.Tests.Integration.Common;
using Xunit;

namespace Persistify.Server.Tests.Integration.Users;

public class GetUserAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task GetUserAsync_WhenUserExists_ReturnsOk()
    {
        // Arrange
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var username = "test";
        var password = "test";
        await CreateUserAsync(username, password);
        var request = new GetUserRequest { Username = username };

        // Act
        var response = await UserService.GetUserAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
    }
}
