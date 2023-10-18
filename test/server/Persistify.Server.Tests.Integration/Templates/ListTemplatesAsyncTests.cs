using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Persistify.Domain.Templates;
using Persistify.Dtos.Common;
using Persistify.Dtos.Templates.Common;
using Persistify.Dtos.Templates.Fields;
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
            Fields = new List<FieldDto>
            {
                new TextFieldDto
                {
                    Name = "TextField1",
                    Required = true,
                    AnalyzerDescriptor = new PresetAnalyzerDescriptorDto
                    {
                        PresetName = "standard"
                    }
                }
            }
        };

        await TemplateService.CreateTemplateAsync(addTemplateRequest, callContext);
        var request = new ListTemplatesRequest { Pagination = new PaginationDto { PageNumber = 0, PageSize = 10 } };

        // Act
        var response = await TemplateService.ListTemplatesAsync(request, callContext);

        // Assert
        response.Should().NotBeNull();
        response.Templates.Should().NotBeNull();
        response.Templates.Should().HaveCount(1);
        response.Templates.First().Name.Should().Be("TestTemplate");
        response.Templates.First().Fields.Should().NotBeNull();
        response.Templates.First().Fields.Should().HaveCount(1);
        response.Templates.First().Fields[0].Name.Should().Be("TextField1");
        response.Templates.First().Fields[0].Required.Should().BeTrue();
    }
}
