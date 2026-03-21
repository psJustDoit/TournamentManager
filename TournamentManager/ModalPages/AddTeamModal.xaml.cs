using System.Windows;
using TournamentManager.ViewModels;
using TournamentManager.Models;
using TournamentManager.Db;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for AddTeamModal.xaml
    /// </summary>
    public partial class AddTeamModal : Window
    {
        private readonly TournamentViewModel _tournamentViewModel;

        public List<Office> Offices { get; set; }
        public AddTeamModal(TournamentViewModel tournamentViewModel)
        {
            _tournamentViewModel = tournamentViewModel;
            Offices = DbRepository.GetAllOffices(); ;

            InitializeComponent();
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

            _tournamentViewModel.IncrementNextTeamCount();

            var selectedOffice = OfficeComboBox.SelectedItem as Office;
            if(selectedOffice == null)
            {
                MessageBox.Show("Poslovnica nije odabrana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var teamToAdd = new Team(_tournamentViewModel.NextTeamId, teamName, false, selectedOffice);


            if (_tournamentViewModel.RoundCount >= 1)
            {
                teamToAdd.IsNewTeam = true;
            }

            _tournamentViewModel.AddTeam(teamToAdd);
            _tournamentViewModel.SortTeamsByScoreDescending();

            TeamNameTextbox.Text = String.Empty;
            OfficeComboBox.SelectedItem = null;
        }
    }
}

