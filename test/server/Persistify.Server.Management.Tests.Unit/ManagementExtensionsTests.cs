using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Persistify.Server.Files;
using Persistify.Server.Management.Managers.Documents;
using Persistify.Server.Management.Managers.PresetAnalyzers;
using Persistify.Server.Management.Managers.Templates;
using Persistify.Server.Management.Managers.Users;
using Persistify.Server.Management.Transactions;
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
        services.Should().HaveCount(12);
        services.Should().Contain(x => x.ImplementationType == typeof(TemplateManager));
        services
            .Should()
            .Contain(x => x.ImplementationType == typeof(IdleTimeoutFileStreamFactory));
        services.Should().Contain(x => x.ImplementationType == typeof(FileHandler));
        services.Should().Contain(x => x.ImplementationType == typeof(LocalFileProvider));
        services
            .Should()
            .Contain(x => x.ImplementationType == typeof(TemplateManagerRequiredFileGroup));
        services.Should().Contain(x => x.ImplementationType == typeof(DocumentManagerStore));
        services
            .Should()
            .Contain(x => x.ImplementationType == typeof(DocumentManagerFileGroupForTemplate));
        services.Should().Contain(x => x.ImplementationType == typeof(TransactionState));
        services.Should().Contain(x => x.ImplementationType == typeof(UserManager));
        services
            .Should()
            .Contain(x => x.ImplementationType == typeof(UserManagerRequiredFileGroup));
        services.Should().Contain(x => x.ImplementationType == typeof(PresetAnalyzerManager));
        services
            .Should()
            .Contain(x => x.ImplementationType == typeof(PresetAnalyzerManagerRequiredFileGroup));
    }
}
