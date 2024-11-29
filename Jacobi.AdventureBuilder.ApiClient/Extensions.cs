﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jacobi.AdventureBuilder.ApiClient;

public static class Extensions
{
    public const string ApiServiceUrl_Setting = "services:apiservice:https:0";

    public static IHostApplicationBuilder AddApiClient(this IHostApplicationBuilder builder)
    {
        var serverUrl = builder.Configuration[ApiServiceUrl_Setting]
            ?? throw new InvalidOperationException($"Missing Configuration '{ApiServiceUrl_Setting}'. Did you call the `WithReference(apiService)` extension method?");

        builder.Services.AddApiClient(serverUrl);
        return builder;
    }

    public static IServiceCollection AddApiClient(this IServiceCollection services, string serverUrl)
    {
        services.AddHttpClient<IAdventureClient, AdventureClient>(client =>
        {
            client.BaseAddress = new Uri(serverUrl);
        });
        return services;
    }
}