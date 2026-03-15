using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for AzurirajTim.xaml
    /// </summary>
    public partial class AzurirajTimModal : Window
    {
        private Team _team;
        private Team _teamCopy;
        public AzurirajTimModal(Team team)
        {
            _team = team;
            _teamCopy = new Team(_team.TeamId, _team.Name, _team.IsDummyTeam, _team.City) 
            {
                Wins = _team.Wins,
                Losses = _team.Losses,
                Score = _team.Score,
                IsDummyTeam = _team.IsDummyTeam,
            };

            InitializeComponent();

            DataContext = _teamCopy;
        }

        private void IncrementTeamWins_Click(object sender, RoutedEventArgs e)
        {
            _teamCopy.Wins += 1;
            _teamCopy.Score += 2;
        }

        private void SubtractTeamWins_Click(object sender, RoutedEventArgs e)
        {
            _teamCopy.Wins -= 1;
            _teamCopy.Score -= 2;
        }

        private void IncrementTeamLosses_Click(object sender, RoutedEventArgs e)
        {
            _teamCopy.Losses += 1;
        }

        private void SubtractTeamLosses_Click(object sender, RoutedEventArgs e)
        {
            _teamCopy.Losses -= 1;
        }

        private void UpdateTeam_Click(object sender, RoutedEventArgs e) 
        {
            _teamCopy.Name = TeamNameTextbox.Text;
            _teamCopy.City = TeamCityTextbox.Text;

            _team.Name = _teamCopy.Name;
            _team.City = _teamCopy.City;
            _team.Wins = _teamCopy.Wins;
            _team.Losses = _teamCopy.Losses;
            _team.Score = _teamCopy.Score;

            DialogResult = true;
        }
    }
}
