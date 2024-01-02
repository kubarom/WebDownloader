// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebDownloader.ConsoleApp;
using WebDownloader.ConsoleApp.ThirdPolishRadio;

using (var host = CreateHostBuilder(args).Build())
{
    await host.StartAsync();
    var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

    // Console.WriteLine("Hello, World!");
    IDiscoveryStrategy discover = host.Services.GetRequiredService<IDiscoveryStrategy>();
    var items = await discover.DiscoverAsync(CancellationToken.None);
    // do work here / get your work service ...

    lifetime.StopApplication();
    await host.WaitForShutdownAsync();
}

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseConsoleLifetime()
        .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning))
        .ConfigureServices((hostContext, services) =>
        {
            services.AddThirdPolishRadio();
            services.AddSingleton(Console.Out);
        });
