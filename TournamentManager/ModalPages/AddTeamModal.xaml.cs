using System.Collections.ObjectModel;
using System.Windows;
using TournamentManager.ViewModels;
using TournamentManager.Models;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for AddTeamModal.xaml
    /// </summary>
    public partial class AddTeamModal : Window
    {
        private readonly TournamentViewModel _tournamentViewModel;
        public AddTeamModal(TournamentViewModel tournamentViewModel)
        {
            InitializeComponent();

            _tournamentViewModel = tournamentViewModel;
        }

        private void AddTeam_Click(object sender, RoutedEventArgs e)
        {
            var teamName = TeamNameTextbox.Text;

            if (String.IsNullOrEmpty(teamName))
            {
                MessageBox.Show("Ime time je prazno", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_tournamentViewModel.AllTeams.Where(t => t.Name == teamName).Any())
            {
                MessageBox.Show("Tim s istim imenom već postoji", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var teamCity = TeamCityTextbox.Text;

            _tournamentViewModel.IncrementNextTeamCount();
            var teamToAdd = new Team(_tournamentViewModel.NextTeamId, teamName, false, teamCity);



            if (_tournamentViewModel.RoundCount >= 1)
            {
                teamToAdd.IsNewTeam = true;
            }

            _tournamentViewModel.AllTeams.Add(teamToAdd);
            _tournamentViewModel.AllTeams = new ObservableCollection<Team>(_tournamentViewModel.AllTeams.OrderByDescending(x => x.TeamTournamentScore));

            TeamNameTextbox.Text = String.Empty;
            TeamCityTextbox.Text = String.Empty;
        }
    }
}

