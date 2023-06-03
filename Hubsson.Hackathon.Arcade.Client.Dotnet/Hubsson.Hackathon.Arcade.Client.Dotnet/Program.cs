using Hubsson.Hackathon.Arcade.Client.Dotnet;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build()
    .GetSection("ArcadeConfig");

var builder = WebApplication.CreateBuilder();
builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<MatchService>();
builder.Services.AddSingleton(config.Get<ArcadeSettings>() ?? new ArcadeSettings());

var app = builder.Build();

await app.StartAsync();

