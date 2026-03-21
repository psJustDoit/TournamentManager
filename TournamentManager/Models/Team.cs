using System.ComponentModel;


namespace TournamentManager.Models
{
    public class Team : INotifyPropertyChanged
    {
        public int TeamId { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        private int _wins;
        public int Wins
        {
            get => _wins;
            set { _wins = value; OnPropertyChanged(nameof(Wins)); }
        }

        private int _losses;
        public int Losses 
        {
            get => _losses; 
            set { _losses = value; OnPropertyChanged(nameof(Losses)); }
        }

        private int _draws;
        public int Draws
        {
            get => _draws;
            set { _draws = value; OnPropertyChanged(nameof(Draws)); }
        }

        // Total team score throughout the tournament
        private int _teamTournamentScore;
        public int TeamTournamentScore 
        {
            get => _teamTournamentScore; 
            set { _teamTournamentScore = value; OnPropertyChanged(nameof(TeamTournamentScore)); }
        }

        // Team score difference
        // Calculated for games such as CS2 where 1 match is played in rounds
        // So match outcome looks something like 6-3, so score differce would be 3 or -3 for that round (depending if the team won more or lost more)
        private int? _scoreDifference;
        public int? ScoreDifference
        {
            get => _scoreDifference;
            set { _scoreDifference = value; OnPropertyChanged(nameof(ScoreDifference)); }
        }

        private Office? _office;
        public Office? Office 
        { 
            get => _office; 
            set { _office = value; OnPropertyChanged(nameof(Office)); } 
        }

        private Team? _opponent;
        public Team? Opponent
        {
            get => _opponent;
            set { _opponent = value; OnPropertyChanged(nameof(Opponent)); }
        }

        private bool? _isWinner;
        public bool? IsWinner
        {
            get => _isWinner;
            set { _isWinner = value; }
        }

        private bool? _isLoser;
        public bool? IsLoser
        {
            get => _isLoser;
            set { _isLoser = value; }
        }

        private bool? _isDraw;
        public bool? IsDraw
        {
            get => _isDraw;
            set { _isDraw = value; }
        }

        private bool? _isKicked;
        public bool? IsKicked
        {
            get => _isKicked;
            set { _isKicked = value; }
        }

        private bool _isDummyTeam;
        public bool IsDummyTeam
        {
            get => _isDummyTeam;
            set { _isDummyTeam = value; }
        }

        private bool _isNewTeam;
        public bool IsNewTeam
        {
            get => _isNewTeam;
            set { _isNewTeam = value; }
        }

        // Used to calculate score difference
        // Entry when the match for both teams is decided
        private int? _gameMatchScore;
        public int? GameMatchScore
        {
            get => _gameMatchScore;
            set { _gameMatchScore = value; }
        }

        public List<int> TeamsIdsAlreadyPlayedWith{ get; set; } = new List<int>();
        public List<int> TeamIdsWonAgainst { get; set; } = new List<int>();

        public Team(int teamId, string name, bool isDummyTeam, Office? office = null)
        {
            TeamId = teamId;
            Name = name;
            Office = office;
            Wins = 0;
            Losses = 0;
            Draws = 0;
            GameMatchScore = null;
            ScoreDifference = 0;
            TeamTournamentScore = 0;
            IsDummyTeam = isDummyTeam;
            Opponent = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void IncreaseTeamWinBy1()
        {
            Wins += 1;
            TeamTournamentScore += 2;
        }

        public void DecreaseTeamWinBy1()
        {
            Wins -= 1;
            TeamTournamentScore -= 2;
        }

        public void IncreaseTeamDrawBy1()
        {
            Draws += 1;
            TeamTournamentScore += 1;
        }

        public void DecreaseTeamDrawBy1()
        {
            Draws -= 1;
            TeamTournamentScore -= 1;
        }

        public void IncreaseTeamLossBy1()
        {
            Losses += 1;
        }

        public void DecreaseTeamLossBy1() 
        {
            Losses -= 1;
        }

        public void IncreaseTeamTournamentScoreBy1()
        {
            TeamTournamentScore += 1;
        }

        public void ResetGameMatchScore()
        {
            GameMatchScore = null;
        }

        public void SetScoreDifference(Team opponent)
        {
            if(opponent == null)
            {
                return;
            }

            ScoreDifference += GameMatchScore - opponent.GameMatchScore;
        }

    }
}
