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
        private bool? isDraw;
        public bool IsDraw { get; set; }
        public List<Team> TeamsPlayedWith { get; set; } = new List<Team>();
        public List<Team> TeamsLostAgainst { get; set; } = new List<Team>();

        public Team(int teamId, string name, string? city = null)
        {
            TeamId = teamId;
            Name = name;
            City = city;
            Wins = 0;
            Losses = 0;
            Draws = 0;
            Score = 0;
            Opponent = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void HandleTeamWin()
        {
            Wins += 1;
            Score += 2;
        }

        public void HandleTeamDraw()
        {
            Draws += 1;
        }

        public void HandleTeamLoss()
        {
            Losses += 1;
        }

        public void AdjustAccidentalTeamWin()
        {
            //TODO
            Wins -= 1;

        }

        //public void SetTeamAndOpponentMatchOutcome(TeamMatchOutcome teamMatchOutcome) 
        //{
        //    switch (teamMatchOutcome) 
        //    {
        //        case TeamMatchOutcome.Win:
        //            Wins += 1;
        //            Score += 2;
        //            Opponent.Losses += 1;
        //            break;
        //        case TeamMatchOutcome.Loss:
        //            Losses += 1;
        //            Opponent.Wins += 1;
        //            break;
        //    }
        //}
    }
}
