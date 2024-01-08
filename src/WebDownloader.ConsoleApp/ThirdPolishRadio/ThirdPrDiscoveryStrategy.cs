// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using static System.StringComparison;

namespace WebDownloader.ConsoleApp.ThirdPolishRadio;

internal class ThirdPrDiscoveryStrategy(IHttpClientFactory httpFactory,
    IDomainObjectFactory objectFactory,
    ILogger<ThirdPrDiscoveryStrategy> logger)
    : IDiscoveryStrategy
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    AsyncRetryPolicy _retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(
            10,
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
        );

    public async Task<List<IDiscoveredItem>> DiscoverAsync(CancellationToken ct)
    {
        var httpClient = httpFactory.CreateClient(Registration.ThirdPrHttpClient);

        const int pageSize = 20;

        var items = new List<IDiscoveredItem>();

        for (int pageIndex = 0; ; pageIndex++)
        {
            string? requestUri = $"https://lp3test.polskieradio.pl/Article/GetListByCategoryId?categoryId=4090&PageSize={pageSize}&skip={pageIndex}&format=400";

            var response = await _retryPolicy.ExecuteAsync(async () => await httpClient.GetAsync(requestUri, ct));

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

            foreach (ThirdPrPageResponseItem pageResponseItem in pageData.Data)
            {
                string relativeUrl = pageResponseItem.GetResourceUrl();

                logger.LogInformation("Downloading '{broacastUrl}' broadcast details", relativeUrl);
                var detailsUrl =
                    $"https://trojka.polskieradio.pl/_next/data/XI7_XRohC52yvsJhLJ5FP/artykul/{relativeUrl}.json";


                var detailsResponse = await _retryPolicy.ExecuteAsync(async () => await httpClient.GetAsync(detailsUrl, ct));

                var detailsRawContent = await detailsResponse.Content.ReadAsStringAsync(ct);

                if (detailsResponse.IsSuccessStatusCode is false)
                {
                    throw new HttpRequestException($"Broadcast details request failed {response.StatusCode} '{detailsRawContent}' to '{detailsUrl}'");
                }

                var options = new JsonDocumentOptions { AllowTrailingCommas = true };
                using JsonDocument doc = JsonDocument.Parse(detailsRawContent, options);

                JsonElement attachmentsItemsElements = doc.RootElement.GetProperty("pageProps")
                    .GetProperty("post")
                    .GetProperty("attachments");

                var attachmentsItems = attachmentsItemsElements.Deserialize<ThirdPrBroadcastResponseItem[]>(_jsonOptions);

                var detailsItem = attachmentsItems?.SingleOrDefault(a => a.FileType.Equals("Audio", InvariantCulture));
                if (detailsItem is null)
                {
                    logger.LogWarning("Broadcast under {resourceUrl},Json has incorrect format. It doesn't contain audio 'pageProps.post.attachments[1]' path", pageResponseItem.GetResourceUrl());
                    continue;
                }

                items.Add(objectFactory.CreateInstance<RecordedBroadcast>(pageResponseItem, detailsItem));
            }
        }

        return items;
    }

    public Task<Stream> DownloadBroadcastAsync(RecordedBroadcast recordedBroadcast, CancellationToken ct)
    {
        var httpClient = httpFactory.CreateClient(Registration.ThirdPrHttpClient);

        return _retryPolicy.ExecuteAsync(() => httpClient.GetStreamAsync(recordedBroadcast.FileUrl, ct));
    }
}
