using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Requests.Users;
using Persistify.Server.Domain.Users;
using Persistify.Server.Tests.Integration.Common;
using Xunit;

namespace Persistify.Server.Tests.Integration.Users;

public class SetPermissionAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task SetPermissionAsync_WhenUserExistsAndSetsPermissionForHimself_ReturnsOk()
    {
        // Arrange
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var username = "test";
        var password = "test";
        await CreateUserAsync(username, password);

        var request = new SetPermissionRequest
        {
            Username = RootCredentials.Username, Permission = (int)Permission.TemplateRead
        };

        // Act
        var response = await UserService.SetPermissionAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
    }
}
