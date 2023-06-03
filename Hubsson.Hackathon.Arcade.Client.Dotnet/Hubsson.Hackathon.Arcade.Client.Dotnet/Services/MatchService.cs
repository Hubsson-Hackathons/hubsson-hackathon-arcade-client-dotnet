using System;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Domain;
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
            _logger.LogError($"CurrentDirection: {_matchRepository.lastDirection}");

            Coordinate head = gameState.players.First(player => player.playerId == _arcadeSettings.TeamId).coordinates.Last();

            Dictionary<Direction, int> freeSpaces = new Dictionary<Direction, int>
            {
                { Direction.Up, 0 },
                { Direction.Down, 0 },
                { Direction.Left, 0 },
                { Direction.Right, 0 }
            };

            if (_matchRepository.lastDirection == null)
            {
                _matchRepository.lastDirection = (Direction)new Random().Next(1, 5);
            }
            else
            {
                Direction oppositeDirection = GetOppositeDirection(_matchRepository.lastDirection.Value);
                if (freeSpaces.ContainsKey(oppositeDirection))
                {
                    freeSpaces.Remove(oppositeDirection);
                }
            }

            foreach (var direction in freeSpaces.Keys.ToList())
            {
                Coordinate newHead = Move(head, direction);

                if (IsValidMove(newHead, gameState))
                {
                    freeSpaces[direction] = CountFreeSpacesInArea(newHead, gameState, 3);
                }
                else
                {
                    freeSpaces[direction] = int.MinValue;
                }
            }

            _matchRepository.lastDirection = freeSpaces.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;

            return new Domain.Action { direction = _matchRepository.lastDirection.Value, iteration = gameState.iteration };

        }



        private Coordinate Move(Coordinate coordinate, Direction direction)
        {
            switch (direction)
            {
                case Direction.Down: return new Coordinate { x = coordinate.x, y = coordinate.y + 1 }; 
                case Direction.Up: return new Coordinate { x = coordinate.x, y = coordinate.y - 1 };
                case Direction.Left: return new Coordinate { x = coordinate.x - 1, y = coordinate.y };
                case Direction.Right: return new Coordinate { x = coordinate.x + 1, y = coordinate.y };
                default: throw new ArgumentException($"Invalid direction: {direction}");
            }
        }


        private bool IsValidMove(Coordinate coordinate, ClientGameState state)
        {
            if (coordinate.x < 0 || coordinate.x > state.width - 1 || coordinate.y < 0 || coordinate.y > state.height - 1)
            {
                return false;
            }

            foreach (var player in state.players)
            {
                if (player.coordinates.Any(c => c.x == coordinate.x && c.y == coordinate.y))
                {
                    return false;
                }
            }

            return true;
        }


        private int CountFreeSpacesInArea(Coordinate coordinate, ClientGameState state, int radius)
        {
            int freeSpaces = 0;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    Coordinate newCoordinate = new Coordinate { x = coordinate.x + dx, y = coordinate.y + dy };
                    if (IsValidMove(newCoordinate, state))
                    {
                        freeSpaces++;
                    }
                }
            }

            return freeSpaces;
        }

        private Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Direction.Down;
                case Direction.Down: return Direction.Up;
                case Direction.Left: return Direction.Right;
                case Direction.Right: return Direction.Left;
                default: throw new ArgumentException($"Invalid direction: {direction}");
            }
        }

        private class MatchRepository
        {
            public Domain.Direction? lastDirection;
            // Write your data fields here what you would like to store between the match rounds
        }
    }
}