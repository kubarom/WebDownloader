// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebDownloader.ConsoleApp;
using WebDownloader.ConsoleApp.ThirdPolishRadio;

using (var host = CreateHostBuilder(args).Build())
{
    var started = DateTime.Now;
    var downloadRootFolder = @"d:\Misc\Temp\TonyZBetonu\";
    await host.StartAsync();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

    var ct = CancellationToken.None;
    var logger = host.Services.GetRequiredService<ILogger<IHostApplicationLifetime>>();

    // Console.WriteLine("Hello, World!");
    IDiscoveryStrategy discover = host.Services.GetRequiredService<IDiscoveryStrategy>();

    var items = await discover.DiscoverAsync(ct);

    logger.LogInformation("Discovered {broadcastsCount} broadcasts", items.Count);

    for (int index = 0; index < items.Count; index++)
    {
        logger.LogInformation("Downloading {index} of {count}", index+1, items.Count);
        IDiscoveredItem discoveredItem = items[index];

        var sw = new Stopwatch();
        sw.Start();

        await using var downloadStream = await discoveredItem.DownloadAsync(ct);

        string filePath = Path.Join(downloadRootFolder, discoveredItem.Name);

        if (Path.Exists(filePath) && new FileInfo(filePath).Length == downloadStream.Length)
        {
            logger.LogWarning("Broadcast {name} already downloaded", discoveredItem.Name);
            return;
        }

        await using var fileStream = File.Open(filePath, FileMode.Create);

        await downloadStream.CopyToAsync(fileStream);

        sw.Stop();
        logger.LogInformation("Downloaded {index} of {count} in {elapsedTime:F2}", index+1, items.Count, sw.Elapsed.TotalSeconds);
    }

    logger.LogInformation($"Completed @{DateTime.Now} in {(DateTime.Now - started).TotalMinutes}");

    lifetime.StopApplication();
    await host.WaitForShutdownAsync();
}

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseConsoleLifetime()
        .ConfigureLogging(builder => builder
            .AddConsole()
            .SetMinimumLevel(LogLevel.Information))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddThirdPolishRadio();
            services.AddSingleton(Console.Out);
            services.AddSingleton<IDomainObjectFactory, DomainObjectFactory>();
        });
