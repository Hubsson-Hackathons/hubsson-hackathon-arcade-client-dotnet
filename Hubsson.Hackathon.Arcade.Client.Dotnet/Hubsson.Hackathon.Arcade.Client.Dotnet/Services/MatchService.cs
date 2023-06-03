using System;
using Hubsson.Hackathon.Arcade.Client.Dotnet.Domain;
using ClientGameState = Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.ClientGameState;

namespace Hubsson.Hackathon.Arcade.Client.Dotnet.Services
{
    public class MatchService
    {
        private MatchRepository _matchRepository;
        private ArcadeSettings _arcadeSettings;
        private HelperService _helperService;
        
        public MatchService(ArcadeSettings settings)
        {
            _matchRepository = new MatchRepository();
            _arcadeSettings = settings;
        }
        
        public void Init()
        {
            _helperService = new HelperService(_arcadeSettings, Direction.Up);
        }

        public Hubsson.Hackathon.Arcade.Client.Dotnet.Domain.Action Update(ClientGameState gameState)
        {
            return _helperService.doNextMove(gameState);
        }

        private class MatchRepository
        {
            // Write your data fields here what you would like to store between the match rounds
        }
    }
}