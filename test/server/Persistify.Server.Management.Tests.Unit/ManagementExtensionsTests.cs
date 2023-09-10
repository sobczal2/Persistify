using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Management.Files;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.Templates;
using Xunit;

namespace Persistify.Server.Management.Tests.Unit;

public class ManagementExtensionsTests
{
    [Fact]
    public void AddManagement_WhenServicesIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        var action = new Action(() =>
        {
            var unused = services.AddManagement();
        });

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddManagement_WhenServicesIsNotNull_AddsServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var actual = services.AddManagement();

        // Assert
        actual.Should().BeSameAs(services);
        services.Should().HaveCount(8);
        services.Should().Contain(x => x.ServiceType == typeof(ITemplateManager));
        services.Should().Contain(x => x.ServiceType == typeof(IFileStreamFactory));
        services.Should().Contain(x => x.ServiceType == typeof(IFileHandler));
        services.Should().Contain(x => x.ServiceType == typeof(IFileProvider));
        services.Should().Contain(x => x.ServiceType == typeof(IRequiredFileGroup));
        services.Should().Contain(x => x.ServiceType == typeof(IDocumentManagerStore));
        services.Should().Contain(x => x.ServiceType == typeof(IFileGroupForTemplate));
    }
}
