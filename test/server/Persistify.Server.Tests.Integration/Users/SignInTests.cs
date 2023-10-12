using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Requests.Users;
using Persistify.Server.Tests.Integration.Common;
using ProtoBuf.Grpc;
using Xunit;

namespace Persistify.Server.Tests.Integration.Users;

public class SignInTests : IntegrationTestBase
{
    public SignInTests(PersistifyServerWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task SignInAsync_WhenCredentialsAreValid_ReturnsOk()
    {
        // Arrange
        var request = new SignInRequest { Username = RootCredentials.Username, Password = RootCredentials.Password };

        // Act
        var response = await UserService.SignInAsync(request, new CallContext());

        // Assert
        response.Should().NotBeNull();
        response.Username.Should().Be(RootCredentials.Username);
        response.AccessToken.Should().NotBeNullOrEmpty();
        response.RefreshToken.Should().NotBeNullOrEmpty();
    }
}
