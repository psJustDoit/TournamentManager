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

        private int nextTeamId = 0;
        private List<Team> Winners { get; set; } = new List<Team>();
        private List<Team> Losers { get; set; } = new List<Team>();
        private List<Team> Draws { get; set; } = new List<Team>();
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
            set { _isViewingWinnersBracket = value; }
        }


        private ObservableCollection<Team> _allTeams = new ObservableCollection<Team>();
        public ObservableCollection<Team> AllTeams
        {
            get => _allTeams;
            set { _allTeams = value; OnPropertyChanged(nameof(AllTeams)); }
        }

        //private ObservableCollection<Team> _nonPlayers = new ObservableCollection<Team>();
        //public ObservableCollection<Team> NonPlayers
        //{
        //    get => _nonPlayers;
        //    set { _nonPlayers = value; OnPropertyChanged(nameof(NonPlayers)); }
        //}

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



        

        public void MatchmakeTeamsFirstRound()
        {
            var teamsNotYetPaired = AllTeams.ToList();
            foreach (var team in AllTeams) 
            {
                if(team.TeamCurrentlyPlayingWith == null)
                {
                    // Pair with dummy team if there are no available teams
                    if(!AllTeams.Where(x => x.TeamId != team.TeamId && x.TeamCurrentlyPlayingWith == null).Any())
                    {
                        dummyTeamCount += 1;
                        NextTeamId += 1;
                        var dummyTeam = new Team(NextTeamId, $"Dummy Team {dummyTeamCount}", null);
                        team.TeamCurrentlyPlayingWith = dummyTeam;
                        dummyTeam.TeamCurrentlyPlayingWith = team;
                        FirstRoundPairings.Add(new TeamPairing(team, dummyTeam));
                    }
                    else
                    {
                        Random random = new Random();
                        var randomTeamPickIndex = random.Next(0, teamsNotYetPaired.Count);
                        var teamToPair = teamsNotYetPaired.Where(x => x.TeamId != team.TeamId).ToList()[randomTeamPickIndex];
                        team.TeamCurrentlyPlayingWith = teamToPair;
                        teamToPair.TeamCurrentlyPlayingWith = team;
                        FirstRoundPairings.Add(new TeamPairing(team, teamToPair));
                        teamsNotYetPaired = teamsNotYetPaired.Where(x => x.TeamCurrentlyPlayingWith == null).ToList();
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

                if(team.TeamCurrentlyPlayingWith == null)
                {
                    if(!teamBracket.Where(x => x.TeamId != team.TeamId && x.TeamCurrentlyPlayingWith == null).Any() && !draws.Any())
                    {
                        if (draws.Any())
                        {
                            opponentTeam = draws.First();
                            team.TeamCurrentlyPlayingWith = opponentTeam;
                            opponentTeam = team;
                            draws.RemoveAt(0);
                        }
                        else
                        {
                            dummyTeamCount += 1;
                            NextTeamId += 1;
                            var opponentDummyTeam = new Team(NextTeamId, $"Dummy Team {dummyTeamCount}", null);
                            team.TeamCurrentlyPlayingWith = opponentDummyTeam;
                            opponentDummyTeam.TeamCurrentlyPlayingWith = team;
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

                        team.TeamCurrentlyPlayingWith = teamWithSmallestScoreDiff;
                        teamWithSmallestScoreDiff.TeamCurrentlyPlayingWith = team;
                        opponentTeam = teamWithSmallestScoreDiff;

                        teamsNotYetPaired = teamsNotYetPaired.Where(x => x.TeamCurrentlyPlayingWith == null).ToList();
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
