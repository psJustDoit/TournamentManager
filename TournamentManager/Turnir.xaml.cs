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

        }

        private void StartTournament_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TeamMatchOutcome_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var matchOutcome = Enum.Parse<TeamMatchOutcome>(button.Tag.ToString());
            var teamPairing = button.DataContext as TeamPairing;

            switch (matchOutcome)
            {
                case TeamMatchOutcome.Team1Win:
                    teamPairing.Team1.HandleTeamWin();
                    teamPairing.Team2.HandleTeamLoss();
                    ColorTeamButtons(button, 'W');
                    break;
                case TeamMatchOutcome.Team2Win:
                    teamPairing.Team2.HandleTeamWin();
                    teamPairing.Team1.HandleTeamLoss();
                    ColorTeamButtons(button, 'W');
                    break;
                case TeamMatchOutcome.Draw:
                    teamPairing.Team1.HandleTeamDraw();
                    teamPairing.Team2.HandleTeamDraw();
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

    }
}
