namespace TournamentManager.HelperClasses
{
    public class RoundHistoryInfo
    {
        public int Round { get; set; }
        public List<TeamScore> Scoreboard {  get; set; }
        public List<TeamPairing> TeamPairing { get; set; }
        public DateTime? DateTimeEnded { get; set; }
    }
}
