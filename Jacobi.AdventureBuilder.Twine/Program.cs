using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Text.Json;
using Jacobi.AdventureBuilder.AdventureModel;
using Jacobi.AdventureBuilder.ApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jacobi.AdventureBuilder.Twine;

public sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        var option = new Option<string>(
            ["--file", "-f"], description: "The path to the Twine JSON file to be processed.");

        var rootCommand = new RootCommand { option };
        rootCommand.Description = "Adventure Builder Twine Processor";
        rootCommand.Handler = CommandHandler.Create<string>(async (file) =>
        {
            if (String.IsNullOrEmpty(file) || !File.Exists(file))
            {
                Console.WriteLine("Please provide a valid file path.");
                return;
            }

            try
            {
                var prog = new Program();
                var json = await File.ReadAllTextAsync(file);
                var world = prog.DoTransform(json);
                await prog.SaveAdventureWorldAsync(world);

                Console.WriteLine($"Adventure World '{world.Name}' ({world.Id}) was saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file: {ex.Message}");
            }
        });

        // Invoke the command line parser
        return await rootCommand.InvokeAsync(args);
    }

    private readonly IConfiguration _configuration;
    private readonly ServiceProvider _services;

    private Program()
    {
        _configuration = BuildConfiguration();

        var services = new ServiceCollection();
        BuildServices(services);
        _services = services.BuildServiceProvider();
    }

    private AdventureWorldInfo DoTransform(string twineJson)
    {
        var twineModel = JsonSerializer.Deserialize<TwineModel>(twineJson);
        var transform = new TwineModelTransform();
        return transform.Transform(twineModel!);
    }

    private async Task SaveAdventureWorldAsync(AdventureWorldInfo adventureWorld)
    {
        var ct = new CancellationToken();
        var apiClient = _services.GetRequiredService<IAdventureClient>();
        await apiClient.UpsertAdventureWorldAsync(adventureWorld, ct);
    }

    private void BuildServices(IServiceCollection services)
    {
        var url = _configuration["ApiServiceUrl"];
        if (String.IsNullOrWhiteSpace(url))
            throw new InvalidOperationException("Configuration settings 'ApiServiceUrl' is missing.");
        services.AddApiClient(url);
    }

    private static IConfiguration BuildConfiguration()
    {
        // Set up configuration sources
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        return configuration;
    }
}
