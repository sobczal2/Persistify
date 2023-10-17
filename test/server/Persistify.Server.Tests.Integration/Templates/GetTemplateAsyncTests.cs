using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Templates;
using Persistify.Requests.Templates;
using Persistify.Server.Tests.Integration.Common;
using Xunit;

namespace Persistify.Server.Tests.Integration.Templates;

public class GetTemplateAsyncTests : IntegrationTestBase
{
    [Fact]
    public async Task GetTemplateAsync_WhenCorrect_ReturnsOk()
    {
        // Arrange
        var callContext = await GetAuthorizedCallContextAsRootAsync();
        var createTemplateRequest = new CreateTemplateRequest
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
        await TemplateService.CreateTemplateAsync(createTemplateRequest, callContext);
        var request = new GetTemplateRequest { TemplateName = "TestTemplate" };

        // Act
        var response = await TemplateService.GetTemplateAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
        response.Template.Should().NotBeNull();
        response.Template.Name.Should().Be("TestTemplate");
        response.Template.TextFields.Should().NotBeNull();
        response.Template.TextFields.Should().HaveCount(1);
        response.Template.TextFields[0].Name.Should().Be("TextField1");
        response.Template.TextFields[0].Required.Should().BeTrue();
        response.Template.TextFields[0].AnalyzerDescriptor.Should().NotBeNull();
        response.Template.TextFields[0].AnalyzerDescriptor.Should().BeOfType<PresetAnalyzerDescriptor>();
        ((PresetAnalyzerDescriptor)response.Template.TextFields[0].AnalyzerDescriptor).PresetName.Should()
            .Be("standard");
    }
}
