namespace Hubsson.Hackathon.Arcade.Client.Dotnet.Contracts
{
    public class JoinMessage
    {
        public string teamId { get; init; }
        
        public string secret { get; init; }
        
        public string name { get; init; }
    }
}