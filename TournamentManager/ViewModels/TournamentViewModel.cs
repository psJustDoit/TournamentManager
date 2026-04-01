using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.TextFormatting;
using TournamentManager.Enums;
using TournamentManager.Extensions;
using TournamentManager.HelperClasses;
using TournamentManager.Models;

namespace TournamentManager.ViewModels
{
    //TODO: add manually entering rounds from 5 - 15
    public enum TournamentState { NotStarted = 1, Started = 2, Finished = 3 }
    public class TournamentViewModel : INotifyPropertyChanged
    {
        private readonly RoundHistoryViewModel _roundHistoryViewModel;

        private int dummyTeamCount = 0;

        //private List<Team> _winners = new List<Team>();
        //public List<Team> Winners
        //{
        //    get => _winners;
        //}

        //private List<Team> _losers = new List<Team>();
        //public List<Team> Losers
        //{
        //    get => _losers;
        //}

        //private List<Team> _draws = new List<Team>();
        //public List<Team> Draws
        //{
        //    get => _draws;
        //}

        //private List<Team> _kickedTeams = new List<Team>();
        //public List<Team> KickedTeams
        //{
        //    get => _kickedTeams;
        //}

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

        public int NextTeamDisplayNumber { get; set; }

        private int _roundCount = 0;
        public int RoundCount
        {
            get => _roundCount;
            set
            {
                if (_roundCount != value)
                {
                    _roundCount = value;
                    OnPropertyChanged(nameof(RoundCount)); ;
                }
            }
        }

        private int? _maxNumberOfRounds;
        public int? MaxNumberOfRounds
        {
            get { return _maxNumberOfRounds; }
            set { _maxNumberOfRounds = value; OnPropertyChanged(nameof(MaxNumberOfRounds)); }
        }

        //private bool _isRoundFinished;
        //public bool IsRoundFinished
        //{
        //    get => _isRoundFinished;
        //    set => _isRoundFinished = value;
        //}

        private DateTime? _tournamentStartDateTime { get; set; }
        public DateTime? TournamentStartDateTime
        {
            get => _tournamentStartDateTime;
            set { _tournamentStartDateTime = value; OnPropertyChanged(nameof(TournamentStartDateTime)); }
        }

        private DateTime? _roundEndDateTime { get; set; }
        public DateTime? RoundEndDateTime
        {
            get => _roundEndDateTime;
            set { _roundEndDateTime = value; OnPropertyChanged(nameof(RoundEndDateTime)); }
        }

        //private bool _isTournamentStarted;
        //public bool IsTournamentStarted
        //{
        //    get => _isTournamentStarted;
        //    set => _isTournamentStarted = value;
        //}

        private TournamentState _tournamentState;
        public TournamentState TournamentState
        {
            get => _tournamentState;
            set => _tournamentState = value;
        }

        private TournamentType? _selectedTournamentType;
        public TournamentType? SelectedTournamentType
        {
            get { return _selectedTournamentType; }
            set { _selectedTournamentType = value; OnPropertyChanged(nameof(SelectedTournamentType)); }
        }

        // Observable properties
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

            TournamentState = TournamentState.NotStarted;

            _roundHistoryViewModel = roundHistoryViewModel;
        }

        private Team FindOpponentForNewlyAddedTeam(Team newTeam)
        {

            var possibleOpponents = AllTeams.Where(t => t.TeamId != newTeam.TeamId).ToList();

            if (RoundCount == 1)
            {
                possibleOpponents = possibleOpponents.Where(t => t.Office?.Id != newTeam.Office?.Id).ToList();
            }

            // Find if there is a team that isnt new that is paired with a dummy team
            var opponentToFind = possibleOpponents.Where(t => t.IsNewTeam == false && t.Opponent != null && t.Opponent.IsDummyTeam == true).FirstOrDefault();
            if (opponentToFind != null) return opponentToFind;

            // Find if there is a newly added team paired with dummy team
            opponentToFind = possibleOpponents.Where(t => t.IsNewTeam == true && t.Opponent != null && t.Opponent.IsDummyTeam == true).FirstOrDefault();
            if (opponentToFind != null) return opponentToFind;

            // Find if there is an available dummy team
            opponentToFind = DummyTeams.Where(dt => dt.Opponent == null).FirstOrDefault();
            if (opponentToFind != null) return opponentToFind;

            // Create dummy team, since no other available teams or dummy teams were found
            opponentToFind = CreateDummyTeam();
            return opponentToFind;
        }


        private TeamPairing? FindTeamPairingByTeamId(int teamId)
        {
            var teamPairToFind = TeamPairings.Where(tp => tp.Team1.TeamId == teamId || tp.Team2.TeamId == teamId).FirstOrDefault();
            return teamPairToFind;
        }

        public void IncrementRound()
        {
            RoundCount += 1;
        }

        public void IncrementNextTeamId()
        {
            NextTeamId += 1;
        }

        public void IncrementNextTeamDisplayNumber()
        {
            NextTeamDisplayNumber += 1;
        }

        public void HandleAllTeamsWinsLossesDrawsAndScores()
        {
            // Set score differences
            foreach (var pairing in TeamPairings)
            {
                if (pairing.Team1 != null && pairing.Team1.IsDummyTeam == false)
                {
                    pairing.Team1.SetScoreDifference(pairing.Team1MatchScore, pairing.Team2MatchScore);
                }

                if (pairing.Team2 != null && pairing.Team2.IsDummyTeam == false)
                {
                    pairing.Team2.SetScoreDifference(pairing.Team2MatchScore, pairing.Team1MatchScore);
                }
            }

            foreach (var team in AllTeams)
            {
                switch (team.TeamMatchOutcomeCurrent)
                {
                    case TeamMatchOutcomeEnum.Winner:
                        team.IncreaseTeamWinBy1();
                        team.TeamMatchOutcomePrevious = TeamMatchOutcomeEnum.Winner;
                        break;
                    case TeamMatchOutcomeEnum.Loser:
                        team.IncreaseTeamLossBy1();
                        team.TeamMatchOutcomePrevious = TeamMatchOutcomeEnum.Loser;
                        break;
                    case TeamMatchOutcomeEnum.Draw:
                        team.IncreaseTeamDrawBy1();
                        team.TeamMatchOutcomePrevious = TeamMatchOutcomeEnum.Draw;
                        break;
                }

                if (team.Opponent != null && team.Opponent.IsDummyTeam == false)
                {
                    team.TeamsIdsAlreadyPlayedWith.Add(team.Opponent.TeamId);
                }

                team.Opponent = null;
            }

            foreach (var dummy in DummyTeams)
            {
                dummy.Opponent = null;
                dummy.TeamMatchOutcomePrevious = dummy.TeamMatchOutcomeCurrent;
            }
        }

        public void ResetRound()
        {
            TeamPairings.Clear();

            foreach (var team in AllTeams)
            {
                team.Opponent = null;
                team.IsNewTeam = false;
            }

            foreach (var dummy in DummyTeams)
            {
                dummy.Opponent = null;
            }

            if (RoundCount == 1)
            {
                MatchmakeTeamsFirstRound();
            }
            else
            {
                MatchmakeTeams();
            }
        }


        // Matchmaking methods
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
                        // Check in case a tournament was restarted and some dummy teams were created before
                        var dummyTeam = DummyTeams.Where(dt => dt.Opponent == null).FirstOrDefault();
                        if (dummyTeam == null)
                        {
                            dummyTeam = CreateDummyTeam();
                            team.Opponent = dummyTeam;
                            dummyTeam.Opponent = team;

                        }
                        else
                        {
                            team.Opponent = dummyTeam;
                            dummyTeam.Opponent = team;
                        }

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


        public void MatchmakeNewlyAddedTeam(Team newTeam)
        {
            var opponent = FindOpponentForNewlyAddedTeam(newTeam);
            var pairingOfOpponent = FindTeamPairingByTeamId(opponent.TeamId);

            if (pairingOfOpponent == null)
            {
                newTeam.Opponent = opponent;
                opponent.Opponent = newTeam;
                TeamPairings.Add(new TeamPairing(newTeam, opponent));
            }
            else
            {
                pairingOfOpponent.Team1.Opponent = null;
                pairingOfOpponent.Team2.Opponent = null;

                pairingOfOpponent.Team1 = newTeam;
                pairingOfOpponent.Team2 = opponent;

                newTeam.Opponent = opponent;
                opponent.Opponent = newTeam;
            }
        }

        public void MatchmakeTeams()
        {
            foreach (var team in AllTeams)
            {
                if (team.Opponent != null || team.IsKicked == true)
                {
                    continue;
                }

                Team? opponent = null;

                var possibleOpponentsQuery = AllTeams.Where(t => t.TeamId != team.TeamId)
                    .Where(t => t.IsKicked != true)
                    .Where(t => t.Opponent == null)
                    .Where(t => !team.TeamsIdsAlreadyPlayedWith.Contains(t.TeamId))
                    .AsQueryable();

                if (team.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Winner)
                {
                    possibleOpponentsQuery = possibleOpponentsQuery.Where(t => t.IsNewTeam == true || t.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Draw || t.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Winner).AsQueryable();
                }

                if (team.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Loser)
                {
                    possibleOpponentsQuery = possibleOpponentsQuery.Where(t => t.IsNewTeam == true || t.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Draw || t.TeamMatchOutcomePrevious == TeamMatchOutcomeEnum.Loser).AsQueryable();
                }

                var possibleOpponentsList = possibleOpponentsQuery.ToList();
                possibleOpponentsList.Shuffle();

                // Find team with smallest score difference to current team
                if (possibleOpponentsList.Any())
                {
                    int smallestScoreDifference = Math.Abs(team.TeamTournamentScore - possibleOpponentsList.First().TeamTournamentScore);
                    opponent = possibleOpponentsList.First();

                    foreach (var possibleOpponent in possibleOpponentsList)
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
                    if (!DummyTeams.Any() || !DummyTeams.Where(dt => dt.Opponent == null).Any())
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

                team.TeamMatchOutcomeCurrent = null;
                opponent.TeamMatchOutcomeCurrent = null;

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
        // End matchmaking methods

        public void RestartTournament()
        {

            AllTeams.Clear();
            TeamPairings.Clear();
            DummyTeams.Clear();

            NextTeamId = 0;
            NextTeamDisplayNumber = 0;
            dummyTeamCount = 0;
            RoundCount = 0;
            TournamentStartDateTime = null;
            MaxNumberOfRounds = null;
            TeamPairings.Clear();
            TeamScoreboardListing.Clear();
            SelectedTournamentType = null;
            TournamentState = TournamentState.NotStarted;
        }

        public bool HasAnyUnresolvedPairs()
        {
            var unresolvedPairOutcomes = TeamPairings.Where(tp => tp.Team1 != null && tp.Team2 != null)
               .Where(tp => tp.Team1.TeamMatchOutcomeCurrent == null)
               .Where(tp => tp.Team2.TeamMatchOutcomeCurrent == null)
               .ToList();

            if (unresolvedPairOutcomes.Any())
            {
                return true;
            }

            return false;
        }

        public void SortTeamsForScoreboard()
        {
            // Sort teams by Score descending, then teams who have same Score will be sorted by ScoreDifference
            var newTeamOrdering = AllTeams.OrderByDescending(x => x.TeamTournamentScore).ThenByDescending(x => x.ScoreDifference).ToList();
            TeamScoreboardListing.Clear();

            for (int i = 0; i < newTeamOrdering.Count(); i++)
            {
                var team = newTeamOrdering[i];
                TeamScoreboardListing.Add(new TeamScoreboardListing(team.TeamId, i + 1, team.TeamDisplayName, team.TeamTournamentScore, team.ScoreDifference));
            }

        }

        public Team CreateDummyTeam()
        {
            dummyTeamCount += 1;
            NextTeamId += 1;
            var dummyTeam = new Team(teamId: NextTeamId, teamDisplayNumber: null, name: $"Dummy Team {dummyTeamCount}", isDummyTeam: true, isNewTeam: false);
            DummyTeams.Add(dummyTeam);
            return dummyTeam;
        }

        // Use this method to add teams instead of adding to collection directly
        public Team AddTeam(string teamName, Office office, bool isNewTeam)
        {
            IncrementNextTeamId();
            IncrementNextTeamDisplayNumber();
            var teamToAdd = new Team(teamId: NextTeamId, teamDisplayNumber: NextTeamDisplayNumber, name: teamName, isDummyTeam: false, isNewTeam: false, office: office);
            AllTeams.Add(teamToAdd);
            var teamScoreboardListing = new TeamScoreboardListing(teamToAdd.TeamId, TeamScoreboardListing.Count + 1, teamToAdd.TeamDisplayName, teamToAdd.TeamTournamentScore, teamToAdd.ScoreDifference);
            TeamScoreboardListing.Add(teamScoreboardListing);

            return teamToAdd;
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
