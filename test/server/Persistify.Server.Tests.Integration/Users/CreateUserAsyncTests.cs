using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Users;
using Persistify.Requests.Users;
using Persistify.Server.Tests.Integration.Common;
using Xunit;

namespace Persistify.Server.Tests.Integration.Users;

public class CreateUserAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateUserAsync_WhenCredentialsAreValid_ReturnsOkAndCreatesUser()
    {
        // Arrange
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var request = new CreateUserRequest { Username = "test", Password = "test" };

        // Act
        var response = await UserService.CreateUserAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
        var getUserResponse =
            await UserService.GetUserAsync(new GetUserRequest { Username = request.Username }, callContext);
        getUserResponse.Should().NotBeNull();
        getUserResponse.Username.Should().Be(request.Username);
        getUserResponse.Permission.Should().Be((int)Permission.None);
    }
}
