using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TournamentManager.ViewModels;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for DodajTimModal.xaml
    /// </summary>
    public partial class DodajTimModal : Window
    {
        private readonly TournamentViewModel _tournamentViewModel;
        public DodajTimModal(TournamentViewModel tournamentViewModel)
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

            _tournamentViewModel.NextTeamId += 1;
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
