using System.ComponentModel;
using TournamentManager.Models;

namespace TournamentManager.HelperClasses
{
    public class TeamPairing : INotifyPropertyChanged
    {
        private Team? _team1 { get; set; }
        public Team? Team1
        {
            get => _team1;
            set { _team1 = value; OnPropertyChanged(nameof(Team1)); }
        }

        private Team? _team2 { get; set; }
        public Team? Team2
        {
            get => _team2;
            set { _team2 = value; OnPropertyChanged(nameof(Team2)); }
        }

        public TeamPairing(Team team1, Team team2) 
        {
            _team1 = team1;
            _team2 = team2;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
