using System;
using System.Threading;
using System.Threading.Tasks;
using Hubsson.Hackathon.Arcade.Client.Dotnet;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Domain;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Action = Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts.Action;
using ClientGameState = Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.ClientGameState;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ArcadeSettings _arcadeSettings;
    private readonly IServiceProvider _serviceProvider;
    
    private MatchService? _match;

    public Worker(ILogger<Worker> logger, ArcadeSettings arcadeSettings, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _arcadeSettings = arcadeSettings;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connected = false;
        while (!connected)
        {
            try
            {
                InitGame();
                connected = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            } 
        }

        await WaitForCancelAsync(stoppingToken);
    }

    private async Task InitGame()
    {
        _logger.LogInformation("Arcade Client worker running at: {time}", DateTimeOffset.Now);
        var socketClient = SocketClient.CreateClient(_arcadeSettings.ServerUrl, _arcadeSettings.Username,
            _arcadeSettings.Secret, _logger);
        SubscribeToUpdates(socketClient);
        await socketClient.ConnectServerAsync(new JoinMessage {
            teamId = _arcadeSettings.TeamId, 
            secret = _arcadeSettings.Secret, 
            name = _arcadeSettings.Username,
        });
    }

    private async Task WaitForCancelAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(500);
        }
    }

    private void SubscribeToUpdates(SocketClient socketClient)
    {
        socketClient.OnGameStart += () =>
        {
            try
            {
                _match = _serviceProvider.GetService<MatchService>();
                _match.Init();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Game init user error: {e.Message}");
            }
        };
            
        socketClient.OnGameUpdate += payload =>
        {
            try
            {
                var domainPayload = new ClientGameState
                {
                    width = payload.width,
                    height = payload.height,
                    iteration = payload.iteration,
                    tickTimeInMs = payload.tickTimeInMs,
                    playerId = payload.playerId,
                    direction = GetEnumDirection(payload.direction),
                    players = payload.players,
                };
                var action = _match.Update(domainPayload);
                
                try
                {
                    var contract = new Action { direction = GetStringDirection(action.direction), iteration = action.iteration };
                    socketClient.SendActionAsync(contract).Wait();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Game action sending error: {e.Message}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Game update user error: {e.Message}");
            }
        };
    }

    private static string GetStringDirection(Direction direction) => direction switch
    {
        Direction.Up => "Up",
        Direction.Down => "Down",
        Direction.Left => "Left",
        Direction.Right => "Right",
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
    
    private static Direction GetEnumDirection(string direction) => direction switch
    {
        "Up" => Direction.Up,
        "Down" => Direction.Down,
        "Left" => Direction.Left,
        "Right" => Direction.Right,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}