// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WebDownloader.ConsoleApp;

public interface IDiscoveredItem
{
    public int Id { get; }
    public string RelativeUri { get; }
    public string Name { get; }
    public string FileUrl { get; }
    Task<Stream> DownloadAsync(CancellationToken ct);
}
