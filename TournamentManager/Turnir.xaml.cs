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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TournamentManager.ViewModels;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for Turnir.xaml
    /// </summary>
    public partial class Turnir : Page
    {
        private readonly TournamentViewModel _tournamentViewModel;
        public Turnir(TournamentViewModel tournamentViewModel)
        {
            _tournamentViewModel = tournamentViewModel;

            var team1 = new Team(_tournamentViewModel.NextTeamId, "tim 1");
            var team2 = new Team(_tournamentViewModel.NextTeamId, "tim 2");
            var team3 = new Team(_tournamentViewModel.NextTeamId, "tim 3");
            var team4 = new Team(_tournamentViewModel.NextTeamId, "tim 4");
            var team5 = new Team(_tournamentViewModel.NextTeamId, "tim 5");
            var team6 = new Team(_tournamentViewModel.NextTeamId, "tim 6");

            _tournamentViewModel.TeamPairings.Add(new TeamPairing(team1, team2));
            _tournamentViewModel.TeamPairings.Add(new TeamPairing(team3, team4));
            _tournamentViewModel.TeamPairings.Add(new TeamPairing(team5, team6));

            InitializeComponent();

            DataContext = _tournamentViewModel;

        }

        private void StartTournament_Click(object sender, RoutedEventArgs e) 
        {

        }

      
    }
}
