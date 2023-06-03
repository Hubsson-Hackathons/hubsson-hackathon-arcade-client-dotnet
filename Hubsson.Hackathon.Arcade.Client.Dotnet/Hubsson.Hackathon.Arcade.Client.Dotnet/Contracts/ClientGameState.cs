namespace Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts
{
    public class ClientGameState
    {
        public int width { get; init; }
        public int height { get; init; }
        public int iteration { get; init; }
        public double tickTimeInMs { get; init; }
        public PlayerCoordinates[] players { get; init; }
    }
}