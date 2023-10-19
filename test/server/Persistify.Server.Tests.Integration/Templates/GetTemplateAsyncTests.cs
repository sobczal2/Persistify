using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Dtos.PresetAnalyzers;
using Persistify.Dtos.Templates.Fields;
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
            Fields = new List<FieldDto>
            {
                new TextFieldDto
                {
                    Name = "TextField1",
                    Required = true,
                    Analyzer = new PresetNameAnalyzerDto
                    {
                        PresetName = "standard"
                    }
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
        response.Template.Fields.Should().NotBeNull();
        response.Template.Fields.Should().HaveCount(1);
        response.Template.Fields[0].Name.Should().Be("TextField1");
        response.Template.Fields[0].Required.Should().BeTrue();
        ((TextFieldDto)response.Template.Fields[0]).Analyzer.Should().NotBeNull();
    }
}
