using Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts;

namespace Hubsson.Hackathon.Arcade.Client.Dotnet.Domain
{
    public class ClientGameState
    {
        public int width { get; init; }
        public int height { get; init; }
        public int iteration { get; init; }
        public double tickTimeInMs { get; init; }
        public string playerId { get; init; }
        public Direction direction { get; init; }
        public PlayerCoordinates[] players { get; init; }
    }
}