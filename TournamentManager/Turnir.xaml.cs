using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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


            InitializeComponent();

            DataContext = _tournamentViewModel;
            UpdateVisibility();
        }

        private void StartTournament_Click(object sender, RoutedEventArgs e)
        {
            if (!_tournamentViewModel.AllTeams.Any())
            {
                MessageBox.Show("Nema dodanih timova.", "No teams", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            _tournamentViewModel.MatchmakeTeamsFirstRound();
            _tournamentViewModel.IncrementRound();
            _tournamentViewModel.RoundStartDateTime = DateTime.Now;
            UpdateVisibility();
        }


        private void NextRound_Click(object sender, RoutedEventArgs e)
        {
            if(_tournamentViewModel.AllTeams.Where(x => x.IsWinner == null && x.IsDraw == null && x.IsLoser == null).Any())
            {
                MessageBox.Show("Postoje parovi gdje rezultat još nije odlučen", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var team in _tournamentViewModel.AllTeams)
            {
                if(team.IsWinner == true)
                {

                }
                if (team.Opponent != null)
                {
                    team.Opponent = null;
                }
            }


            // Matchmake winners with other winners or teams who had a draw
            _tournamentViewModel.MatchmakeTeams();
            _tournamentViewModel.IncrementRound();

            _tournamentViewModel.AllTeams = new ObservableCollection<Team>(_tournamentViewModel.AllTeams.OrderByDescending(x => x.Score));
            UpdateVisibility();
        }

        private void TeamMatchOutcome_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var matchOutcome = Enum.Parse<TeamMatchOutcome>(button.Tag.ToString());
            var teamPairing = button.DataContext as TeamPairing;
            var team1 = teamPairing.Team1;
            var team2 = teamPairing.Team2;

            switch (matchOutcome)
            {
                case TeamMatchOutcome.Team1Win:
                    team1.IsWinner = true;
                    team1.IsDraw = null;
                    team1.IsLoser = null;

                    team2.IsWinner = null;
                    team2.IsDraw = null;
                    team2.IsLoser = true;

                    ColorTeamButtons(button, 'W');
                    break;
                case TeamMatchOutcome.Team2Win:
                    team2.IsWinner = true;
                    team2.IsDraw = null;
                    team2.IsLoser = null;

                    team1.IsWinner = null;
                    team1.IsDraw = null;
                    team1.IsLoser = true;

                    ColorTeamButtons(button, 'W');
                    break;
                case TeamMatchOutcome.Draw:
                    team1.IsWinner = null;
                    team1.IsDraw = true;
                    team1.IsLoser = null;

                    team2.IsWinner = null;
                    team2.IsDraw = true;
                    team2.IsLoser = null;

                    ColorTeamButtons(button, 'D');
                    break;
            }
        }

        private void ColorTeamButtons(Button? button, char matchOutcome)
        {
            // Find which border was clicked
            var clickedBorder = FindParent<Border>(button);
            // Get the parent panel that holds both borders
            var parentPanel = VisualTreeHelper.GetParent(clickedBorder) as Panel;

            // Reset all buttons in both borders first
            foreach (var border in parentPanel.Children.OfType<Border>())
            {
                ResetLabelColors(border);
            }

            // Color the clicked border's label for team match outcome (win=green, draw=yellow)
            HighlightLabel(clickedBorder, matchOutcome, isWinner: true);

            // Color the other border's buttons, if other button is green this one will be red for loss
            var otherBorder = parentPanel.Children.OfType<Border>()
                .FirstOrDefault(b => b != clickedBorder);

            HighlightLabel(otherBorder, matchOutcome, isWinner: false);
        }

        private void HighlightLabel(Border border, char outcome, bool isWinner)
        {
            var label = FindChildren<Label>(border).FirstOrDefault();

            label.Background = outcome switch
            {
                'W' => isWinner && outcome == 'W' ? Brushes.Green : Brushes.Red,
                'D' => Brushes.Yellow,
                _ => Brushes.Transparent
            };
        }

        private void ResetLabelColors(Border border)
        {
            foreach (var button in FindChildren<Label>(border))
            {
                button.BorderBrush = Brushes.Transparent;
            }
        }

        // Find all children of a type recursively
        private static IEnumerable<T> FindChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T match) yield return match;
                foreach (var nested in FindChildren<T>(child))
                    yield return nested;
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent == null) return null;
            if (parent is T match) return match;
            return FindParent<T>(parent);
        }

        private void ViewWinnersBracket_Click(object sender, RoutedEventArgs e) 
        {
            _tournamentViewModel.IsViewingWinnersBracket = true;
            UpdateVisibility();
        }

        private void ViewLosersBracket_Click(object sender, RoutedEventArgs e)
        {
            _tournamentViewModel.IsViewingWinnersBracket = false;
            UpdateVisibility(); 
        }

        private void UpdateVisibility()
        {
            if(_tournamentViewModel.RoundCount == 0)
            {
                StartTournamentButton.Visibility = Visibility.Visible;
                NextRoundButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                StartTournamentButton.Visibility = Visibility.Collapsed;
                NextRoundButton.Visibility = Visibility.Visible;
            }

        }

    }
}
