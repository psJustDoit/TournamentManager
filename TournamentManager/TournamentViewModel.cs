using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager.ViewModels
{
    public class TournamentViewModel : INotifyPropertyChanged
    {
        private uint dummyTeamCount = 0;
        private List<Team> _winners = new List<Team>();
        public List<Team> Winners
        {
            get => _winners;
        }

        private List<Team> _losers = new List<Team>();
        public List<Team> Losers
        {
            get => _losers;
        }

        private List<Team> _draws = new List<Team>();
        public List<Team> Draws
        {
            get => _draws;
        }

        private List<Team> _newlyAddedTeams = new List<Team>();
        public List<Team> NewlyAddedTeams
        {
            get => _newlyAddedTeams;
        }

        private int nextTeamId = 0;
        public int NextTeamId
        {
            get { return nextTeamId; }
            set { nextTeamId = value; }
        }

        private uint _roundCount = 0;
        public uint RoundCount
        {
            get => _roundCount;
            set
            {
                if (_roundCount != value)
                {
                    _roundCount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RoundCount)));
                }
            }
        }

        private bool _isRoundFinished;
        public bool IsRoundFinished
        {
            get => _isRoundFinished;
            set => _isRoundFinished = value;
        }

        private DateTime? _roundStartDateTime { get; set; }
        public DateTime? RoundStartDateTime
        {
            get => _roundStartDateTime;
            set { _roundStartDateTime = value; OnPropertyChanged(nameof(RoundStartDateTime)); }
        }

        private DateTime? _roundEndDateTime { get; set; }
        public DateTime? RoundEndDateTime
        {
            get => _roundEndDateTime;
            set { _roundEndDateTime = value; OnPropertyChanged(nameof(RoundEndDateTime)); }
        }

        private bool _isViewingWinnersBracket = true;
        public bool IsViewingWinnersBracket
        {
            get { return _isViewingWinnersBracket; }
            set { _isViewingWinnersBracket = value; }
        }


        private ObservableCollection<Team> _allTeams = new ObservableCollection<Team>();
        public ObservableCollection<Team> AllTeams
        {
            get => _allTeams;
            set { _allTeams = value; OnPropertyChanged(nameof(AllTeams)); }
        }

        private ObservableCollection<TeamPairing> _winnersBracket = new ObservableCollection<TeamPairing>(); 
        public ObservableCollection<TeamPairing> WinnersBracket 
        { 
            get => _winnersBracket; 
            set { _winnersBracket = value; OnPropertyChanged(nameof(WinnersBracket)); } 
        }

        private ObservableCollection<TeamPairing> _losersBracket = new ObservableCollection<TeamPairing>();
        public ObservableCollection<TeamPairing> LosersBracket
        {
            get => _losersBracket;
            set { _losersBracket = value; OnPropertyChanged(nameof(LosersBracket)); }
        }

        private ObservableCollection<TeamPairing> _teamPairings = new ObservableCollection<TeamPairing>();
        public ObservableCollection<TeamPairing> TeamPairings
        {
            get => _teamPairings;
            set { _teamPairings = value; OnPropertyChanged(nameof(TeamPairings)); }
        }

        public TournamentViewModel()
        {

        }

        public void MatchmakeTeamsFirstRound()
        {
            var teamsNotYetPaired = AllTeams.ToList();
            foreach (var team in AllTeams) 
            {
                if(team.Opponent == null)
                {
                    // Pair with dummy team if there are no available teams
                    if(!AllTeams.Where(x => x.TeamId != team.TeamId && x.Opponent == null).Any())
                    {
                        var dummyTeam = CreateDummyTeam();
                        team.Opponent = dummyTeam;
                        dummyTeam.Opponent = team;
                        TeamPairings.Add(new TeamPairing(team, dummyTeam));
                    }
                    else
                    {
                        Random random = new Random();
                        var listOfPossibleOpponents = teamsNotYetPaired.Where(x => x.TeamId != team.TeamId).ToList();
                        var randomTeamPickIndex = random.Next(0, listOfPossibleOpponents.Count);
                        var teamToPair = listOfPossibleOpponents[randomTeamPickIndex];
                        team.Opponent = teamToPair;
                        teamToPair.Opponent = team;
                        TeamPairings.Add(new TeamPairing(team, teamToPair));
                        teamsNotYetPaired = teamsNotYetPaired.Where(x => x.Opponent == null).ToList();
                    }
                }
            }
        }

        public void IncrementRound()
        {
            RoundCount += 1;
        }

        public void MatchmakeTeams()
        {      
            foreach(var team in AllTeams)
            {
                Team? opponent = null;
                var possibleOpponents = AllTeams.Where(t => t.TeamId != team.TeamId)
                    .Where(t => t.Opponent == null)
                    .Where(t => t.IsDraw == true)
                    .Where(t => t.IsNewTeam == true)
                    .Where(t => !t.IsDummyTeam)
                    .Where(t => !team.TeamsIdsAlreadyPlayedWith.Contains(t.TeamId));

                if(team.IsWinner == true)
                {
                    possibleOpponents = possibleOpponents.Where(t => t.IsWinner == true);
                }

                if (team.IsLoser == true)
                {
                    possibleOpponents = possibleOpponents.Where(t => t.IsLoser == true);
                }

                // Find team with smallest score difference to current team
                int smallestScoreDifference = 0;
                foreach(var possibleOpponent in possibleOpponents)
                {
                    var scoreDifference = Math.Abs(team.Score -  possibleOpponent.Score);
                    if(scoreDifference <= smallestScoreDifference)
                    {
                        opponent = possibleOpponent;
                        smallestScoreDifference = scoreDifference;
                    }
                }

                if(opponent == null)
                {
                    if(!AllTeams.Where(t => t.IsDummyTeam == true).Any())
                    {
                        var dummyTeam = CreateDummyTeam();
                        opponent = dummyTeam;
                    }
                    else
                    {
                        opponent = AllTeams.Where(t => t.IsDummyTeam == true).FirstOrDefault();
                    }
                }

                team.Opponent = opponent;
                opponent.Opponent = team;

                team.IsWinner = null;
                team.IsLoser = null;
                team.IsDraw = null;

                opponent.IsWinner = null;
                opponent.IsLoser = null;
                opponent.IsDraw = null;

                if(opponent.IsNewTeam == true)
                {
                    opponent.IsNewTeam = false;
                }

                if (team.IsNewTeam == true) 
                {
                    team.IsNewTeam = false;
                }
                
                var teamPair = new TeamPairing(team, opponent);

                TeamPairings.Add(teamPair);
            }         
          
        }

        public void SortTeamsByScoreDescending()
        {
            var sortedList = new ObservableCollection<Team>(AllTeams.OrderByDescending(t => t.Score));
            AllTeams = sortedList;
        }

        public Team CreateDummyTeam()
        {
            dummyTeamCount += 1;
            NextTeamId += 1;
            var dummyTeam = new Team(NextTeamId, $"Dummy Team {dummyTeamCount}", true);
            AllTeams.Add(dummyTeam);
            return dummyTeam;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
