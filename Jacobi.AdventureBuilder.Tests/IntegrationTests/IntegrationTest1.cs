using Aspire.Hosting;

namespace Jacobi.AdventureBuilder.Tests.IntegrationTests;

public abstract class IntegrationTest
{
    protected async Task<DistributedApplication> StartDistributedApplicationAsync()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Jacobi_AdventureBuilder_AppHost>();
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
        // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging

        var app = await appHost.BuildAsync();
        await app.StartAsync();

        return app;
    }
}

public class IntegrationTest1 : IntegrationTest
{
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        await using var app = await StartDistributedApplicationAsync();
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

        // Act
        var httpClient = app.CreateHttpClient("webfrontend");
        await resourceNotificationService.WaitForResourceAsync("webfrontend", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
        var response = await httpClient.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
