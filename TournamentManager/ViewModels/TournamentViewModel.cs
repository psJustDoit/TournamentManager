using System.Collections.ObjectModel;
using System.ComponentModel;
using TournamentManager.HelperClasses;
using TournamentManager.Models;

namespace TournamentManager.ViewModels
{
    //TODO: add manually entering rounds from 5 - 15
    public class TournamentViewModel : INotifyPropertyChanged
    {
        private readonly RoundHistoryViewModel _roundHistoryViewModel;

        private int dummyTeamCount = 0;
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

        private List<Team> _dummyTeams = new List<Team>();
        public List<Team> DummyTeams
        {
            get => _dummyTeams;
        }

        private List<TournamentType> _tournamentType = new List<TournamentType>();
        public List<TournamentType> TournamentType
        {
            get => _tournamentType;
        }

        //private List<TeamScore> _scoreboard = new List<TeamScore>();
        //public List<TeamScore> Scoreboard
        //{
        //    get => _scoreboard;
        //    set => _scoreboard = value;
        //}

        private int nextTeamId = 0;
        public int NextTeamId
        {
            get { return nextTeamId; }
            set { nextTeamId = value; }
        }

        private int _roundCount = 0;
        public int RoundCount
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

        private TournamentType _selectedTournamentType;
        public TournamentType SelectedTournamentType
        {
            get { return _selectedTournamentType; }
            set { _selectedTournamentType = value;  }
        }

        private ObservableCollection<Team> _allTeams = new ObservableCollection<Team>();
        public ObservableCollection<Team> AllTeams
        {
            get => _allTeams;
            set { _allTeams = value; OnPropertyChanged(nameof(AllTeams)); }
        }

        private ObservableCollection<TeamPairing> _teamPairings = new ObservableCollection<TeamPairing>();
        public ObservableCollection<TeamPairing> TeamPairings
        {
            get => _teamPairings;
            set { _teamPairings = value; OnPropertyChanged(nameof(TeamPairings)); }
        }

        private ObservableCollection<TeamScoreboardListing> _teamScoreboardListing = new ObservableCollection<TeamScoreboardListing>();
        public ObservableCollection<TeamScoreboardListing> TeamScoreboardListing
        {
            get => _teamScoreboardListing;
            set { _teamScoreboardListing = value; OnPropertyChanged(nameof(TeamScoreboardListing)); }
        }

        public TournamentViewModel(RoundHistoryViewModel roundHistoryViewModel)
        {
            TournamentType.Add(new TournamentType(1, "CS 5v5"));
            TournamentType.Add(new TournamentType(2, "CS Wingman"));
            TournamentType.Add(new TournamentType(3, "Valorant 5v5"));
            TournamentType.Add(new TournamentType(4, "Valorant Skirmish"));
            TournamentType.Add(new TournamentType(5, "Siege 5v5"));
            TournamentType.Add(new TournamentType(6, "LoL 5v5"));
            TournamentType.Add(new TournamentType(7, "LoL ARAM"));
            TournamentType.Add(new TournamentType(8, "Fortnite"));
            TournamentType.Add(new TournamentType(9, "PUBG Battle Royal"));

            _roundHistoryViewModel = roundHistoryViewModel;
        }

        public void MatchmakeTeamsFirstRound()
        {
            var teamsNotYetPaired = AllTeams.ToList();
            foreach (var team in AllTeams)
            {
                if (team.Opponent == null)
                {
                    // Pair with dummy team if there are no available teams
                    if (!AllTeams.Where(t => t.TeamId != team.TeamId && t.Opponent == null && t.Office.Id != team.Office.Id).Any())
                    {
                        var dummyTeam = CreateDummyTeam();
                        team.Opponent = dummyTeam;
                        dummyTeam.Opponent = team;
                        TeamPairings.Add(new TeamPairing(team, dummyTeam));
                    }
                    else
                    {
                        Random random = new Random();
                        var listOfPossibleOpponents = teamsNotYetPaired.Where(t => t.TeamId != team.TeamId && t.Office.Id != team.Office.Id).ToList();
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

        public void IncrementNextTeamCount()
        {
            NextTeamId += 1;
        }

        public void  MatchmakeTeamsNext()
        {
            foreach (var team in AllTeams)
            {
                if(team.Opponent != null)
                {
                    continue;
                }

                Team? opponent = null;
                var possibleOpponents = AllTeams.Where(t => t.TeamId != team.TeamId)
                    .Where(t => t.IsKicked != true)
                    .Where(t => t.Opponent == null)
                    .Where(t => !team.TeamsIdsAlreadyPlayedWith.Contains(t.TeamId));

                if (team.IsWinner == true)
                {
                    possibleOpponents = possibleOpponents.Where(t => t.IsDraw == true || t.IsNewTeam == true || t.IsWinner == true).ToList();
                }

                if (team.IsLoser == true)
                {
                    possibleOpponents = possibleOpponents.Where(t => t.IsDraw == true || t.IsNewTeam == true || t.IsLoser == true).ToList();
                }

                // Find team with smallest score difference to current team
                if (possibleOpponents.Any())
                {
                    int smallestScoreDifference = Math.Abs(team.TeamTournamentScore - possibleOpponents.First().TeamTournamentScore);
                    opponent = possibleOpponents.First();

                    foreach (var possibleOpponent in possibleOpponents)
                    {
                        var scoreDifference = Math.Abs(team.TeamTournamentScore - possibleOpponent.TeamTournamentScore);
                        if (scoreDifference <= smallestScoreDifference)
                        {
                            opponent = possibleOpponent;
                            smallestScoreDifference = scoreDifference;
                        }
                    }
                }

                if (opponent == null)
                {
                    if(!DummyTeams.Any() || !DummyTeams.Where(dt => dt.Opponent == null).Any())
                    {
                        opponent = CreateDummyTeam();
                    }
                    else
                    {
                        opponent = DummyTeams.Where(dt => dt.Opponent == null).FirstOrDefault();
                    }  
                }

                team.Opponent = opponent;
                opponent.Opponent = team;

                team.IsWinner = null;
                team.IsLoser = null;
                team.IsDraw = null;
                team.ResetGameMatchScore();
                

                opponent.IsWinner = null;
                opponent.IsLoser = null;
                opponent.IsDraw = null;
                opponent.ResetGameMatchScore();

                if (opponent.IsNewTeam == true)
                {
                    opponent.IsNewTeam = false;
                }

                if (team.IsNewTeam == true)
                {
                    team.IsNewTeam = false;
                }

                TeamPairings.Add(new TeamPairing(team, opponent));
            }
        }

        public void SortTeamsByScoreDescending()
        {
            var newTeamOrdering = AllTeams.OrderByDescending(x => x.TeamTournamentScore).ToList();
            TeamScoreboardListing.Clear();

            for (int i = 0; i < newTeamOrdering.Count(); i++) 
            {
                var team = newTeamOrdering[i];
                TeamScoreboardListing.Add(new TeamScoreboardListing(i + 1, team.Name, team.TeamTournamentScore, team.ScoreDifference));
            }
                
        }

        public Team CreateDummyTeam()
        {
            dummyTeamCount += 1;
            NextTeamId += 1;
            var dummyTeam = new Team(NextTeamId, $"Dummy Team {dummyTeamCount}", true);
            DummyTeams.Add(dummyTeam);
            return dummyTeam;
        }

        // Use this method to add teams instead of adding to collection directly
        public void AddTeam(Team teamToAdd)
        {
            AllTeams.Add(teamToAdd);
            var teamScoreboardListing = new TeamScoreboardListing(TeamScoreboardListing.Count + 1, teamToAdd.Name, teamToAdd.TeamTournamentScore, teamToAdd.ScoreDifference);
            TeamScoreboardListing.Add(teamScoreboardListing);
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
