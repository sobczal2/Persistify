using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Templates;
using Persistify.Requests.Common;
using Persistify.Requests.Templates;
using Persistify.Server.Tests.Integration.Common;
using Xunit;

namespace Persistify.Server.Tests.Integration.Templates;

public class ListTemplatesAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task ListTemplatesAsync_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var addTemplateRequest = new CreateTemplateRequest
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

        await TemplateService.CreateTemplateAsync(addTemplateRequest, callContext);
        var request = new ListTemplatesRequest { Pagination = new Pagination { PageNumber = 0, PageSize = 10 } };

        // Act
        var response = await TemplateService.ListTemplatesAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
        response.Templates.Should().NotBeNull();
        response.Templates.Should().HaveCount(1);
        response.Templates.First().Name.Should().Be("TestTemplate");
        response.Templates.First().TextFields.Should().NotBeNull();
        response.Templates.First().TextFields.Should().HaveCount(1);
        response.Templates.First().TextFields[0].Name.Should().Be("TextField1");
        response.Templates.First().TextFields[0].Required.Should().BeTrue();
        response.Templates.First().TextFields[0].AnalyzerDescriptor.Should().NotBeNull();
        response.Templates.First().TextFields[0].AnalyzerDescriptor.Should().BeOfType<PresetAnalyzerDescriptor>();
        ((PresetAnalyzerDescriptor)response.Templates.First().TextFields[0].AnalyzerDescriptor).PresetName.Should()
            .Be("standard");
    }
}
