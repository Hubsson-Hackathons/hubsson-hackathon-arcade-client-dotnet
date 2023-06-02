namespace Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts
{
    public class PlayerCoordinates
    {
        public string playerId { get; init; }
        
        public Coordinate[] coordinates { get; init; }
    }

    public class Coordinate
    {
        public int x { get; init; }
        
        public int y { get; init; }
    }
}