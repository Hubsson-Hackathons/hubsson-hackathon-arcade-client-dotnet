using System.Net;
using Hubsson.Hackathon.Arcade.Client.Dotnet;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .GetSection("ArcadeConfig");
        
        services.AddHostedService<Worker>();
        services.AddTransient<MatchService>();
        services.AddSingleton(config.Get<ArcadeSettings>() ?? new ArcadeSettings());
    })
    .Build();

await host.RunAsync();