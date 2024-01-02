// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WebDownloader.ConsoleApp.ThirdPolishRadio;

internal class RecordedBroadcast : IDiscoveredItem
{
    public required int Id { get; init; }
    public required string Url { get; init; }
}
