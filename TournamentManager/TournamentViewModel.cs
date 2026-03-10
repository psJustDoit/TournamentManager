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
        private List<Team> _winners { get; set; } = new List<Team>();
        public List<Team> Winners
        {
            get;
            private set;
        }

        private List<Team> _losers { get; set; } = new List<Team>();
        public List<Team> Losers
        {
            get;
            private set;
        }

        private List<Team> _draws { get; set; } = new List<Team>();
        public List<Team> Draws
        {
            get;
            private set;
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

        private bool _isViewingWinnersBracket;
        public bool IsViewingWinnersBracket
        {
            get { return _isViewingWinnersBracket; }
            set { _isViewingWinnersBracket = value; OnPropertyChanged(nameof(IsViewingWinnersBracket)); }
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

        private ObservableCollection<TeamPairing> _firstRoundPairings = new ObservableCollection<TeamPairing>();
        public ObservableCollection<TeamPairing> FirstRoundPairings
        {
            get => _firstRoundPairings;
            set { _firstRoundPairings = value; OnPropertyChanged(nameof(FirstRoundPairings)); }
        }

        public TournamentViewModel()
        {
            var team1 = new Team(NextTeamId, "tim 1");
            var team2 = new Team(NextTeamId, "tim 2");
            var team3 = new Team(NextTeamId, "tim 3");
            var team4 = new Team(NextTeamId, "tim 4");
            var team5 = new Team(NextTeamId, "tim 5");
            var team6 = new Team(NextTeamId, "tim 6");

            FirstRoundPairings.Add(new TeamPairing(team1, team2));
            team1.Opponent = team2;
            team2.Opponent = team1;
            FirstRoundPairings.Add(new TeamPairing(team3, team4));
            team3.Opponent = team4;
            team4.Opponent = team3;
            FirstRoundPairings.Add(new TeamPairing(team5, team6));
            team5.Opponent = team6;
            team5.Opponent = team6;

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
                        dummyTeamCount += 1;
                        NextTeamId += 1;
                        var dummyTeam = new Team(NextTeamId, $"Dummy Team {dummyTeamCount}", null);
                        team.Opponent = dummyTeam;
                        dummyTeam.Opponent = team;
                        FirstRoundPairings.Add(new TeamPairing(team, dummyTeam));
                    }
                    else
                    {
                        Random random = new Random();
                        var randomTeamPickIndex = random.Next(0, teamsNotYetPaired.Count);
                        var teamToPair = teamsNotYetPaired.Where(x => x.TeamId != team.TeamId).ToList()[randomTeamPickIndex];
                        team.Opponent = teamToPair;
                        teamToPair.Opponent = team;
                        FirstRoundPairings.Add(new TeamPairing(team, teamToPair));
                        teamsNotYetPaired = teamsNotYetPaired.Where(x => x.Opponent == null).ToList();
                    }
                }
            } 
        }


        public void MatchmakeTeams(List<Team> teamBracket, List<Team> draws, bool isWinnersBracket)
        {
            var teamsNotYetPaired = teamBracket;
            foreach(var team in teamBracket)
            {
                Team opponentTeam = null;

                if(team.Opponent == null)
                {
                    if(!teamBracket.Where(x => x.TeamId != team.TeamId && x.Opponent == null).Any() && !draws.Any())
                    {
                        if (draws.Any())
                        {
                            opponentTeam = draws.First();
                            team.Opponent = opponentTeam;
                            opponentTeam = team;
                            draws.RemoveAt(0);
                        }
                        else
                        {
                            dummyTeamCount += 1;
                            NextTeamId += 1;
                            var opponentDummyTeam = new Team(NextTeamId, $"Dummy Team {dummyTeamCount}", null);
                            team.Opponent = opponentDummyTeam;
                            opponentDummyTeam.Opponent = team;
                            opponentTeam = opponentDummyTeam;
                        }                          
                    }
                    else
                    {
                        var possibleOpponents = teamsNotYetPaired
                            .Where(x => x.TeamId != team.TeamId)
                            .Where(x => !team.TeamsPlayedWith.Select(t => t.TeamId).Contains(x.TeamId))
                            .ToList();

                        Team? teamWithSmallestScoreDiff = null;
                        int smallestScoreDiff = 0;
                        // Find team with smallest score difference
                        foreach (var possibleOpponent in possibleOpponents)
                        {
                            var scoreDiff = Math.Abs(possibleOpponent.Score - team.Score);
                            if (scoreDiff <= smallestScoreDiff)
                            {
                                smallestScoreDiff = scoreDiff;
                                teamWithSmallestScoreDiff = possibleOpponent;
                            }
                        }

                        team.Opponent = teamWithSmallestScoreDiff;
                        teamWithSmallestScoreDiff.Opponent = team;
                        opponentTeam = teamWithSmallestScoreDiff;

                        teamsNotYetPaired = teamsNotYetPaired.Where(x => x.Opponent == null).ToList();
                    }
                }

                if (isWinnersBracket)
                {
                    WinnersBracket.Add(new TeamPairing(team, opponentTeam));
                }
                else
                {
                    LosersBracket.Add(new TeamPairing(team, opponentTeam));
                }
            }
        }

        public void SortTeamsByScoreDescending()
        {
            var sortedList = new ObservableCollection<Team>(AllTeams.OrderByDescending(t => t.Score));
            AllTeams = sortedList;
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
