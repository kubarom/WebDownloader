// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using WebDownloader.ConsoleApp.ThirdPolishRadio;

namespace WebDownloader.ConsoleApp;

public interface IDiscoveryStrategy
{
    Task<List<IDiscoveredItem>> DiscoverAsync(CancellationToken ct);
    Task<Stream> DownloadBroadcastAsync(RecordedBroadcast recordedBroadcast, CancellationToken ct);
}
