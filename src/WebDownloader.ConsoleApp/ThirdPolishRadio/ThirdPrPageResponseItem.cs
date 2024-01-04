// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WebDownloader.ConsoleApp.ThirdPolishRadio;

public class ThirdPrPageResponseItem
{
    public required int Id { get; set; }
    public required string Url { get; set; }

    internal string GetResourceUrl()
    {
        return Url.Split("/", StringSplitOptions.RemoveEmptyEntries).Last();
    }
}
