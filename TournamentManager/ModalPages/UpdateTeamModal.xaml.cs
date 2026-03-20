using System.Windows;
using TournamentManager.Models;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for UpdateTeam.xaml
    /// </summary>
    public partial class UpdateTeamModal : Window
    {
        private Team _team;
        private Team _teamCopy;
        public UpdateTeamModal(Team team)
        {
            _team = team;
            _teamCopy = new Team(_team.TeamId, _team.Name, _team.IsDummyTeam, _team.City) 
            {
                Wins = _team.Wins,
                Losses = _team.Losses,
                TeamTournamentScore = _team.TeamTournamentScore,
                IsDummyTeam = _team.IsDummyTeam,
            };

            InitializeComponent();

            DataContext = _teamCopy;
        }

        private void IncrementTeamWins_Click(object sender, RoutedEventArgs e)
        {
            //_teamCopy.IncreaseTeamWin();
            _teamCopy.IncreaseTeamWinBy1();
        }

        private void SubtractTeamWins_Click(object sender, RoutedEventArgs e)
        {
            //_teamCopy.DecreaseTeamWin();
            _teamCopy.DecreaseTeamWinBy1();
        }

        private void IncrementTeamLosses_Click(object sender, RoutedEventArgs e)
        {
            //_teamCopy.Losses += 1;
            _team.IncreaseTeamLossBy1();
        }

        private void SubtractTeamLosses_Click(object sender, RoutedEventArgs e)
        {
            //_teamCopy.Losses -= 1;
            _team.DecreaseTeamLossBy1();
        }

        private void UpdateTeam_Click(object sender, RoutedEventArgs e) 
        {
            _teamCopy.Name = TeamNameTextbox.Text;
            _teamCopy.City = TeamCityTextbox.Text;

            _team.Name = _teamCopy.Name;
            _team.City = _teamCopy.City;
            _team.Wins = _teamCopy.Wins;
            _team.Losses = _teamCopy.Losses;
            _team.TeamTournamentScore = _teamCopy.TeamTournamentScore;

            DialogResult = true;
        }
    }
}
