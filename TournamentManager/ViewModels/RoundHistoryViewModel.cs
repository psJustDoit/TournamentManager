using System.ComponentModel;
using TournamentManager.HelperClasses;
using TournamentManager.Models;

namespace TournamentManager.ViewModels
{
    public class RoundHistoryViewModel : INotifyPropertyChanged
    {
        // Used to make a list of clickable elements in RoundHistory page
        private List<int> _rounds = new List<int>();
        public List<int> Rounds
        {
            get { return _rounds; }
            set { _rounds = value; }
        }

        private int? _selectedRound;
        public int? SelectedRound
        {
            get { return _selectedRound; }
            set { _selectedRound = value; OnPropertyChanged(nameof(SelectedRound)); }
        }

        // History of played rounds
        private List<RoundHistoryInfo> _allRounds = new List<RoundHistoryInfo>();
        public List<RoundHistoryInfo> AllRounds
        {
            get { return _allRounds; }
            set { _allRounds = value; }
        }

        private RoundHistoryInfo _roundToView;
        public RoundHistoryInfo RoundToView
        {
            get { return _roundToView; }
            set { _roundToView = value; OnPropertyChanged(nameof(RoundToView)); }
        }

        public void SetRoundToView(int roundCount)
        {
            SelectedRound = roundCount;
            
            var roundInfo = AllRounds.Where(x => x.Round == SelectedRound).FirstOrDefault();
            if (roundInfo != null) 
            {
                RoundToView = roundInfo;
            }
        }

        public void CreateRoundHistoryEntry(TournamentViewModel tournamentViewModel)
        {
            var roundHistoryInfo = new RoundHistoryInfo();
            var teamPairingsHistoryInfo = new List<TeamPairing>();
            foreach (var teamPairing in tournamentViewModel.TeamPairings)
            {
                Team? team1 = null;
                if (teamPairing.Team1 != null)
                {
                    team1 = new Team(teamPairing.Team1.TeamId, teamPairing.Team1.Name, teamPairing.Team1.IsDummyTeam);
                    team1.IsDraw = teamPairing.Team1.IsDraw;
                    team1.IsLoser = teamPairing.Team1.IsLoser;
                    team1.IsWinner = teamPairing.Team1.IsWinner;
                    team1.TeamTournamentScore = teamPairing.Team1.TeamTournamentScore;
                    team1.GameMatchScore = teamPairing.Team1.GameMatchScore;
                    team1.ScoreDifference = teamPairing.Team1.ScoreDifference;
                    team1.Wins = teamPairing.Team1.Wins;
                    team1.Losses = teamPairing.Team1.Losses;
                    team1.Draws = teamPairing.Team1.Draws;
                    team1.TeamsIdsAlreadyPlayedWith = teamPairing.Team1.TeamsIdsAlreadyPlayedWith;
                    team1.TeamIdsWonAgainst = teamPairing.Team1.TeamIdsWonAgainst;
                }

                Team? team2 = null;
                if (teamPairing.Team2 != null)
                {
                    team2 = new Team(teamPairing.Team2.TeamId, teamPairing.Team2.Name, teamPairing.Team2.IsDummyTeam);
                    team2.IsDraw = teamPairing.Team2.IsDraw;
                    team2.IsLoser = teamPairing.Team2.IsLoser;
                    team2.IsWinner = teamPairing.Team2.IsWinner;
                    team2.TeamTournamentScore = teamPairing.Team2.TeamTournamentScore;
                    team2.GameMatchScore = teamPairing.Team2.GameMatchScore;
                    team2.ScoreDifference = teamPairing.Team2.ScoreDifference;
                    team2.Wins = teamPairing.Team2.Wins;
                    team2.Losses = teamPairing.Team2.Losses;
                    team2.Draws = teamPairing.Team2.Draws;
                    team2.TeamsIdsAlreadyPlayedWith = teamPairing.Team2.TeamsIdsAlreadyPlayedWith;
                    team2.TeamIdsWonAgainst = teamPairing.Team2.TeamIdsWonAgainst;
                }

                team1?.Opponent = team2;
                team2?.Opponent = team1;

                var pairing = new TeamPairing(team1, team2);

                teamPairingsHistoryInfo.Add(pairing);
            }

            var currentScoreboard = tournamentViewModel.TeamScoreboardListing.ToList();

            roundHistoryInfo.Round = tournamentViewModel.RoundCount;
            roundHistoryInfo.Scoreboard = currentScoreboard;
            roundHistoryInfo.TeamPairing = teamPairingsHistoryInfo;
            roundHistoryInfo.DateTimeEnded = DateTime.Now;

            // Add round info to round histories
            Rounds.Add(tournamentViewModel.RoundCount);
            AllRounds.Add(roundHistoryInfo);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
