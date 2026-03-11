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

        private ObservableCollection<TeamPairing> _firstRoundPairings = new ObservableCollection<TeamPairing>();
        public ObservableCollection<TeamPairing> FirstRoundPairings
        {
            get => _firstRoundPairings;
            set { _firstRoundPairings = value; OnPropertyChanged(nameof(FirstRoundPairings)); }
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
                        var listOfPossibleOpponents = teamsNotYetPaired.Where(x => x.TeamId != team.TeamId).ToList();
                        var randomTeamPickIndex = random.Next(0, listOfPossibleOpponents.Count);
                        var teamToPair = listOfPossibleOpponents[randomTeamPickIndex];
                        team.Opponent = teamToPair;
                        teamToPair.Opponent = team;
                        FirstRoundPairings.Add(new TeamPairing(team, teamToPair));
                        teamsNotYetPaired = teamsNotYetPaired.Where(x => x.Opponent == null).ToList();
                    }
                }
            }
        }


        public void MatchmakeTeams(List<Team> teamBracket, List<Team> draws, List<Team> newlyAddedTeams, bool isWinnersBracket)
        {      
            var teamsNotYetPaired = teamBracket;
            foreach(var team in teamBracket.OrderByDescending(x => x.Score))
            {
                Team opponentTeam = null;

                if(team.Opponent == null)
                {
                    if(!teamBracket.Where(x => x.TeamId != team.TeamId && x.Opponent == null).Any() && !draws.Any())
                    {
                        if (draws.Any())
                        {
                            // If team was not paired from a team in their bracket, try pairing with a team who had a draw 
                            opponentTeam = draws.First();
                            team.Opponent = opponentTeam;
                            opponentTeam = team;
                            draws.RemoveAt(0);
                        }
                        else if (newlyAddedTeams.Any())
                        {
                            // If team was not paired from a team in their bracket and there was no teams with draw results, try pairing with a newly added team
                            opponentTeam = draws.First();
                            team.Opponent = opponentTeam;
                            opponentTeam = team;
                            newlyAddedTeams.RemoveAt(0);
                        }
                        else
                        {
                            // Pair with dummy team if no other opponents are found
                            dummyTeamCount += 1;
                            NextTeamId += 1;
                            var opponentDummyTeam = new Team(NextTeamId, $"Dummy Team {dummyTeamCount}", true, null);
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
                            .Where(x => !x.IsDummyTeam)
                            .ToList();

                        Team? teamWithSmallestScoreDiff = null;
                        int smallestScoreDiff = 0;
                        // Find team with closest score to matchmake with
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

                        // Reset match outcome values for both teams
                        team.IsWinner = null;
                        team.IsLoser = null;
                        team.IsDraw = null;

                        opponentTeam.IsWinner = null;
                        opponentTeam.IsLoser = null;
                        opponentTeam.IsDraw = null;

                        teamsNotYetPaired = teamsNotYetPaired.Where(x => x.Opponent == null).ToList();
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

                if (!teamsNotYetPaired.Any())
                {
                    return;
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
