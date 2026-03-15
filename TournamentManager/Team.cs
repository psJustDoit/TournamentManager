using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
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

        private uint _wins;
        public uint Wins
        {
            get => _wins;
            set { _wins = value; OnPropertyChanged(nameof(Wins)); }
        }

        private uint _losses;
        public uint Losses 
        {
            get => _losses; 
            set { _losses = value; OnPropertyChanged(nameof(Losses)); }
        }

        private uint _draws;
        public uint Draws
        {
            get => _draws;
            set { _draws = value; OnPropertyChanged(nameof(Draws)); }
        }

        private int _score;
        public int Score 
        {
            get => _score; 
            set { _score = value; OnPropertyChanged(nameof(Score)); }
        }

        private string? _city;
        public string? City 
        { 
            get => _city; 
            set { _city = value; OnPropertyChanged(nameof(City)); } 
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

        public List<int> TeamsIdsAlreadyPlayedWith{ get; set; } = new List<int>();
        public List<int> TeamsIdsLostAgainst { get; set; } = new List<int>();

        public Team(int teamId, string name, bool isDummyTeam, string? city = null)
        {
            TeamId = teamId;
            Name = name;
            City = city;
            Wins = 0;
            Losses = 0;
            Draws = 0;
            Score = 0;
            IsDummyTeam = isDummyTeam;
            Opponent = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void IncreaseTeamWin()
        {
            Wins += 1;
            Score += 2;
        }

        public void DecreaseTeamWin()
        {
            Wins -= 1;
            Score -= 2;
        }

        public void IncreaseTeamDraw()
        {
            Draws += 1;
            Score += 1;
        }

        public void DecreaseTeamDraw()
        {
            Draws -= 1;
            Score -= 1;
        }

        public void IncreaseTeamLoss()
        {
            Losses += 1;
        }

        public void DecreaseTeamLoss() 
        {
            Losses -= 1;
        }

    }
}
