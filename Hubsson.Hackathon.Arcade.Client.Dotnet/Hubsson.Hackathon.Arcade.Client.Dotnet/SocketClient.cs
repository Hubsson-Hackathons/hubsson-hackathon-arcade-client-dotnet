using System;
using System.Threading.Tasks;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts;
using Microsoft.Extensions.Logging;
using SocketIOClient;
using SocketIOClient.Transport;
using Action = System.Action;

namespace Hubsson.Hackathon.Arcade.Client.Dotnet
{
    public class SocketClient
    {
        private const string START = "start";
        private const string UPDATE = "update";
        
        private SocketIO _socket;
        private readonly ILogger<Worker> _logger;

        public Action OnGameStart;

        public Action<ClientGameState?> OnGameUpdate;
        
        private SocketClient(string serverUrl, string username, string secret, ILogger<Worker> logger)
        {
            _logger = logger;
            _socket = new SocketIO(serverUrl, new SocketIOOptions
            {
                Reconnection = true,
                ReconnectionAttempts = 100,
                ReconnectionDelay = 50,
                Auth = new Auth { username = username, secret = secret},
                Transport = TransportProtocol.WebSocket,
            });
            this.initSocket();
        }

        public static SocketClient CreateClient(string serverUrl, string username, string secret, ILogger<Worker> logger)
        {
            return new SocketClient(serverUrl, username, secret, logger);
        }
        
        public async Task ConnectServerAsync(JoinMessage message)
        {
            _socket.OnConnected += (sender, _) =>
            {
                _logger.LogInformation($"Socket connected: {sender} | {DateTimeOffset.Now}");
                JoinLobbyAsync(message).Wait();
            };
            await _socket.ConnectAsync();
        }

        public async Task SendActionAsync(Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts.Action message)
        {
            _logger.LogInformation($"Sending Game Action: {message} | {DateTimeOffset.Now}");
            await _socket.EmitAsync("action", message);
        }
        
        private async Task JoinLobbyAsync(JoinMessage message)
        {
            _logger.LogInformation($"Joining Lobby: {message} | {DateTimeOffset.Now}");
            await _socket.EmitAsync("join", message);
        }

        private void initSocket()
        {
            setupGenericCallbacks();
            _socket.On(START, _ =>
            {
                _logger.LogInformation($"Game start arrived");
                OnGameStart?.Invoke();
            });
            _socket.On(UPDATE, response =>
            {
                var value = deserializeResult<ClientGameState>(response);
                _logger.LogInformation($"Game update arrived: {value}");
                OnGameUpdate?.Invoke(value);
            });
        }

        private void setupGenericCallbacks()
        {
            _socket.OnError += (_, error) => _logger.LogError($"Socket error: {error} | {DateTimeOffset.Now}");
            _socket.OnReconnectAttempt += (_, attempt) => _logger.LogWarning($"Socket reconnection attempt: {attempt} | {DateTimeOffset.Now}");
            _socket.OnReconnectError += (_, error) => _logger.LogError($"Socket reconnection error: {error} | {DateTimeOffset.Now}");
            _socket.OnReconnected += (_, attempt) => _logger.LogInformation($"Socket reconnected: {attempt} | {DateTimeOffset.Now}");
            _socket.OnPing += (_, args) => _logger.LogInformation($"Socket ping: {args} | {DateTimeOffset.Now}");
            _socket.OnPong += (_, timeSpan) => _logger.LogInformation($"Socket pong, timespan: {timeSpan} | {DateTimeOffset.Now}");
        }

        private T? deserializeResult<T>(SocketIOResponse response)
        {
            try
            {
                return response.GetValue<T>();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Deserialization error: {e.Message}");
                return default;
            }
        }
        
        private class Auth
        {
            public string username { get; init; }

            public string secret { get; init; }
        }
    }
}