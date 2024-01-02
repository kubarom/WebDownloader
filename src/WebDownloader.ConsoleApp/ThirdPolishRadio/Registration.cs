// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.DependencyInjection;

namespace WebDownloader.ConsoleApp.ThirdPolishRadio;

internal static class Registration
{
    internal const string ThirdPrHttpClient = "ThirdPolishRadio";

    public static IServiceCollection AddThirdPolishRadio(this IServiceCollection services)
    {
        services
            .AddSingleton<IDiscoveryStrategy, ThirdPrDiscoveryStrategy>()
            .AddHttpClient(ThirdPrHttpClient, options =>
            {
                options.DefaultRequestHeaders.Add("X-Api-Key", "f2d29c85-ab02-4cee-ba4f-403c5f8f344d");
            });

        return services;
    }
}
