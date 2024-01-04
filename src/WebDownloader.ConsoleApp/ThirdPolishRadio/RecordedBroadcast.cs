// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WebDownloader.ConsoleApp.ThirdPolishRadio;

public class RecordedBroadcast(ThirdPrPageResponseItem responseItem, ThirdPrBroadcastResponseItem responseDetail, IDiscoveryStrategy discoveryStrategy) : IDiscoveredItem
{
    public required int Id { get; init; } = responseItem.Id;
    public required string RelativeUri { get; init; } = responseItem.GetResourceUrl();
    public required string Name { get; init; } = responseDetail.FileName;
    public required string FileUrl { get; init; } = responseDetail.File;

    public Task<Stream> DownloadAsync(CancellationToken ct)
    {
        return discoveryStrategy.DownloadBroadcastAsync(this, ct);
    }
}
