// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WebDownloader.ConsoleApp.ThirdPolishRadio;

internal class ThirdPrDiscoveryStrategy(IHttpClientFactory httpFactory, ILogger<ThirdPrDiscoveryStrategy> logger)
    : IDiscoveryStrategy
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<IDiscoveredItem>> DiscoverAsync(CancellationToken ct)
    {
        var httpClient = httpFactory.CreateClient(Registration.ThirdPrHttpClient);

        const int pageSize = 20;

        var items = new List<IDiscoveredItem>();

        for (int pageIndex = 0; ; pageIndex++)
        {
            string? requestUri = $"https://lp3test.polskieradio.pl/Article/GetListByCategoryId?categoryId=4090&PageSize={pageSize}&skip={pageIndex}&format=400";
            var response = await httpClient.GetAsync(requestUri, ct);

            logger.LogInformation($"Getting page #{pageIndex} of 3PR 'Tony z Betonu' broadcasts");

            string rawContent = await response.Content.ReadAsStringAsync(ct);

            if (response.IsSuccessStatusCode is false)
            {
                throw new HttpRequestException($"Request failed {response.StatusCode} '{rawContent}' to '{requestUri}'");
            }

            var pageData = JsonSerializer.Deserialize<ThirdPrPageResponse>(rawContent, _jsonOptions);
            if ((pageData?.Data.Any() ?? false) is false)
            {
                break;
            }

            items.AddRange(pageData.Data.Select(pd => new RecordedBroadcast { Id = pd.Id, Url = pd.GetResourceUrl() }));
        }

        return items;
    }
}
