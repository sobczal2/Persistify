using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Server.Tests.Integration.Common;
using Xunit;

namespace Persistify.Server.Tests.Integration.Templates;

public class CreateTemplateAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateTemplateAsync_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var request = new CreateTemplateRequest
        {
            TemplateName = "TestTemplate",
            TextFields = new List<TextField>
            {
                new()
                {
                    Name = "TextField1",
                    Required = true,
                    AnalyzerDescriptor = new PresetAnalyzerDescriptor { PresetName = "standard" }
                }
            }
        };

        // Act
        var response = await TemplateService.CreateTemplateAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
    }
}
