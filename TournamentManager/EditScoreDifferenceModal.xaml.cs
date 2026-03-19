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
    /// Interaction logic for EditScoreDifferenceModal.xaml
    /// </summary>
    public partial class EditScoreDifferenceModal : Window
    {
        private readonly TeamPairing _pairing;
        public EditScoreDifferenceModal(TeamPairing teamPairing)
        {
            InitializeComponent();
            _pairing = teamPairing;
            DataContext = _pairing;

            Team1GameMatchScore.Text = _pairing.Team1.GameMatchScore?.ToString();
            Team2GameMatchScore.Text = _pairing.Team2.GameMatchScore?.ToString();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {

            if (Team1GameMatchScore.Text == null || Team1GameMatchScore.Text == "" || Team1GameMatchScore.Text.IsWhiteSpace())
            {
                MessageBox.Show($"Prazna vrijednost za tim {_pairing.Team1.Name}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Team2GameMatchScore.Text == null || Team2GameMatchScore.Text == "" || Team2GameMatchScore.Text.IsWhiteSpace())
            {
                MessageBox.Show($"Prazna vrijednost za tim {_pairing.Team2.Name}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var team1MatchScore = Convert.ToInt32(Team1GameMatchScore.Text);
            var team2MatchScore = Convert.ToInt32(Team2GameMatchScore.Text);

            _pairing.Team1.GameMatchScore = team1MatchScore;
            _pairing.Team2.GameMatchScore = team2MatchScore;

            this.Close();
        }
    }
}
