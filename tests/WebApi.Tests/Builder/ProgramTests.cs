using System.Net;

namespace WebApi.Tests.Builder;

public class ProgramTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    [Fact]
    public async Task Program_Builder_ShouldStartAndReturnOkOnSwagger()
    {
        // Act
        var response = await _httpClient.GetAsync("/swagger");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
