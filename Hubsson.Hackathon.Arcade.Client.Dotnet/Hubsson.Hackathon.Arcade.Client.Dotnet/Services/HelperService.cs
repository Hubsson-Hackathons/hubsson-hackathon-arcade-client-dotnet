using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Domain;
using ClientGameState = Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts.ClientGameState;

namespace Hubsson.Hackathon.Arcade.Client.Dotnet.Services
{
    public class HelperService
    {
        private readonly ArcadeSettings _settings;
        private Direction _curDirection;
        public HelperService(ArcadeSettings settings, Direction curDirection)
        {
            _settings = settings;
            _curDirection = curDirection;
        }

        private bool IsValidMove()
        {
            return true;
        }

        public bool CanMoveInDirection(int targetX, int targetY, int width, int height, Coordinate[] obstacles)
        {
            return targetX >= 0 &&
                   targetX < width &&
                   targetY >= 0 &&
                   targetY < height
                   && !obstacles.Contains(new Coordinate() { x = targetX, y = targetY });
        }

        public Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.Action doNextMove(Domain.ClientGameState _gameState)
        {
            var currentPosition = _gameState.players.Where(x => x.playerId == _settings.Username)
                .Select(x => x.coordinates);
            var iteration = _gameState.iteration;
            

            switch (_curDirection)
            {
                case Direction.Up:

                    _curDirection = Direction.Right;
                    return new Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.Action()
                    {
                        direction = Direction.Right,
                        iteration = iteration,
                    };
                case Direction.Down:

                    _curDirection = Direction.Left;
                    return new Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.Action()
                    {
                        direction = Direction.Left,
                        iteration = iteration,
                    };
                case Direction.Left:

                    _curDirection = Direction.Down;
                    return new Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.Action()
                    {
                        direction = Direction.Down,
                        iteration = iteration,
                    };
                case Direction.Right:
                    _curDirection = Direction.Up;
                    return new Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.Action()
                    {
                        direction = Direction.Up,
                        iteration = iteration,
                    };
            }

            // placeholder
            return new Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.Action()
            {
                direction = Direction.Up,
                iteration = 0,
            };
        }
    }
}
