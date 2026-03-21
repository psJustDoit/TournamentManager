namespace TournamentManager.HelperClasses
{
    public class TeamScoreboardListing
    {
        public int TeamIndex { get; set; }
        public string Name { get; set; }
        public int TournamentScore { get; set; }
        public int? ScoreDifference { get; set; }

        public TeamScoreboardListing(int teamIndex, string name, int tournamentScore, int? scoreDifference)
        {
            TeamIndex = teamIndex;
            Name = name;
            TournamentScore = tournamentScore;
            ScoreDifference = scoreDifference;
        }
    }
}
