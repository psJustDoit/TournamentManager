using System.Windows;
using TournamentManager.Models;
using TournamentManager.ViewModels;
using TournamentManager.Db;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for UpdateTeam.xaml
    /// </summary>
    public partial class EditTeamModal : Window
    {
        private readonly TournamentViewModel _tournamentViewModel;
        private Team _team;
        private Team _teamCopy;
        public List<Office> Offices { get; set; }

        public EditTeamModal(Team team, TournamentViewModel tournamentViewModel)
        {
            _tournamentViewModel = tournamentViewModel;
            _team = team;
            _teamCopy = new Team(teamId: _team.TeamId, name: _team.Name, isDummyTeam: _team.IsDummyTeam, office: _team.Office)
            {
                Wins = _team.Wins,
                Losses = _team.Losses,
                Draws = _team.Draws,
                TeamTournamentScore = _team.TeamTournamentScore,
            };

            DataContext = _teamCopy;
            Offices = DbRepository.GetAllOffices();

            OfficeComboBox.SelectedItem = _team.Office;

            InitializeComponent();
        }

        private void IncrementTeamWins_Click(object sender, RoutedEventArgs e)
        {
            _teamCopy.IncreaseTeamWinBy1();
        }

        private void SubtractTeamWins_Click(object sender, RoutedEventArgs e)
        {
            _teamCopy.DecreaseTeamWinBy1();
        }

        private void IncrementTeamLosses_Click(object sender, RoutedEventArgs e)
        {
            _team.IncreaseTeamLossBy1();
        }

        private void SubtractTeamLosses_Click(object sender, RoutedEventArgs e)
        {
            _team.DecreaseTeamLossBy1();
        }

        private void IncrementTeamDraws_Click(object sender, RoutedEventArgs e)
        {
            _team.IncreaseTeamDrawBy1();
        }

        private void SubtractTeamDraws_Click(object sender, RoutedEventArgs e)
        {
            _team.DecreaseTeamDrawBy1();
        }

        private void UpdateTeam_Click(object sender, RoutedEventArgs e) 
        {
            if(TeamNameTextbox.Text.IsWhiteSpace() || TeamNameTextbox == null || TeamLossesTextbox.Text == null)
            {
                MessageBox.Show("Ime time ne može biti prazno", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Check if there is another team with the same name that is trying to be set
            if(_tournamentViewModel.AllTeams.Where(t => t.TeamId != _team.TeamId && t.Name == TeamNameTextbox.Text).Any())
            {
                MessageBox.Show("Drugi tim s istim imenom već postoji", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OfficeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Poslovnica ne može biti prazna", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _teamCopy.Name = TeamNameTextbox.Text;
            _teamCopy.Office = OfficeComboBox.SelectedItem as Office;

            _team.Name = _teamCopy.Name;
            _team.Office = _teamCopy.Office;
            _team.Wins = _teamCopy.Wins;
            _team.Losses = _teamCopy.Losses;
            _team.Draws = _teamCopy.Draws;
            _team.TeamTournamentScore = _teamCopy.TeamTournamentScore;

            DialogResult = true;
        }
    }
}
