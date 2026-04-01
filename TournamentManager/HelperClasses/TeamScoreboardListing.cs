using System.Reflection.Metadata.Ecma335;

namespace TournamentManager.HelperClasses
{
    public class TeamScoreboardListing
    {
        public int TeamId { get; set; }
        public int TeamPosition { get; set; }
        public string TeamDisplayName { get; set; }
        public int TournamentScore { get; set; }
        public int? ScoreDifference { get; set; }

        public TeamScoreboardListing(int teamId, int teamPosition, string teamDisplayNumber, int tournamentScore, int? scoreDifference)
        {
            TeamId = teamId;
            TeamPosition = teamPosition;
            TeamDisplayName = teamDisplayNumber;
            TournamentScore = tournamentScore;
            ScoreDifference = scoreDifference;
        }
    }
}
