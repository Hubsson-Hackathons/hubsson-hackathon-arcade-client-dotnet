using System;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts;
using ClientGameState = Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.ClientGameState;

namespace Hubsson.Hackathon.Arcade.Client.Dotnet.Services
{
    public class MatchService
    {
        private MatchRepository _matchRepository;
        private ArcadeSettings _arcadeSettings;
        private readonly ILogger<MatchService> _logger;

        public MatchService(ArcadeSettings settings, ILogger<MatchService> logger)
        {
            _matchRepository = new MatchRepository();
            _arcadeSettings = settings;
            _logger = logger;
        }
        
        public void Init()
        {
            // On Game Init
            throw new NotImplementedException();
        }

        public Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.Action Update(ClientGameState gameState)
        {
            _logger.LogInformation($"{gameState}");
            var action = new Domain.Action();
            Coordinate coords = getMyCoordinates(gameState);
            if (coords.x == 1)
            {
                // Bal széle
                if (_matchRepository.currentDirection == Domain.Direction.Down || coords.y > gameState.height / 2)
                {
                    _logger.LogError("Turning up");
                    _matchRepository.currentDirection = Domain.Direction.Up;
                    return new Domain.Action
                    {
                        direction = Domain.Direction.Up,
                        iteration = gameState.iteration,
                    };
                }
                else
                {
                    _logger.LogError("Turning down");
                    _matchRepository.currentDirection = Domain.Direction.Down;
                    return new Domain.Action
                    {
                        direction = Domain.Direction.Down,
                        iteration = gameState.iteration,
                    };
                }
            }
            if (coords.x == gameState.width - 1)
            {
                // jobb széle
                if (_matchRepository.currentDirection == Domain.Direction.Down || coords.y > gameState.height / 2)
                {
                    _logger.LogError("Turning up");
                    _matchRepository.currentDirection = Domain.Direction.Up;
                    return new Domain.Action
                    {
                        direction = Domain.Direction.Up,
                        iteration = gameState.iteration,
                    };
                }
                else
                {
                    _logger.LogError("Turning down");
                    _matchRepository.currentDirection = Domain.Direction.Down;
                    return new Domain.Action
                    {
                        direction = Domain.Direction.Down,
                        iteration = gameState.iteration,
                    };
                }
            }
            if (coords.y == 1)
            {
                // teteje
                if (_matchRepository.currentDirection == Domain.Direction.Right || coords.x > gameState.width / 2)
                {
                    _logger.LogError("Turning left");
                    _matchRepository.currentDirection = Domain.Direction.Left;
                    return new Domain.Action
                    {
                        direction = Domain.Direction.Left,
                        iteration = gameState.iteration,
                    };
                }
                else
                {
                    _logger.LogError("Turning right");
                    _matchRepository.currentDirection = Domain.Direction.Right;
                    return new Domain.Action
                    {
                        direction = Domain.Direction.Right,
                        iteration = gameState.iteration,
                    };
                }
            }
            if (coords.y == gameState.height - 1)
            {
                // alja
                if (_matchRepository.currentDirection == Domain.Direction.Left || coords.x > gameState.width / 2)
                {
                    _logger.LogError("Turning left");
                    _matchRepository.currentDirection = Domain.Direction.Left;
                    return new Domain.Action
                    {
                        direction = Domain.Direction.Left,
                        iteration = gameState.iteration,
                    };
                }
                else
                {
                    _logger.LogError("Turning right");
                    _matchRepository.currentDirection = Domain.Direction.Right;
                    return new Domain.Action
                    {
                        direction = Domain.Direction.Right,
                        iteration = gameState.iteration,
                    };
                }
            }
            _matchRepository.currentDirection = Domain.Direction.Down;
            return new Domain.Action
            {
                direction = Domain.Direction.Down,
                iteration = gameState.iteration,
            };
        }

        private Coordinate getMyCoordinates(ClientGameState gameState)
        {
            return gameState.players.FirstOrDefault(player => player.playerId == _arcadeSettings.TeamId).coordinates.Last();
        }

        private class MatchRepository
        {
            public Domain.Direction currentDirection;
            // Write your data fields here what you would like to store between the match rounds
        }
    }
}