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
            _tournamentViewModel.RoundCount += 1;
            _tournamentViewModel.RoundStartDateTime = DateTime.Now;
            _tournamentViewModel.IsRoundFinished = false;
            UpdateVisibility();
        }

        private void RoundDone_Click(object sender, RoutedEventArgs e)
        {
            if (_tournamentViewModel.AllTeams.Where(x => x.IsWinner == null && x.IsDraw == null && x.IsLoser == null).Any())
            {
                MessageBox.Show("Postoje timovi kojima rezultat još nije odlučen.", "Nesvrstani timovi", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_tournamentViewModel.IsRoundFinished)
            {
                MessageBox.Show("Runda je već gotova", "Gotova runda", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_tournamentViewModel.RoundCount == 1)
            {
                foreach (var teamPairing in _tournamentViewModel.FirstRoundPairings)
                {
                    var team1 = teamPairing.Team1;
                    var team2 = teamPairing.Team2;

                    team1.Opponent = null;
                    team2.Opponent = null;

                    team1.TeamsPlayedWith.Add(team2);
                    team2.TeamsPlayedWith.Add(team1);

                    //Team1
                    if (team1.IsWinner == true)
                    {
                        _tournamentViewModel.Winners.Add(team1);
                    }

                    if (team1.IsLoser == true)
                    {
                        _tournamentViewModel.Losers.Add(team1);
                    }

                    if (team1.IsDraw == true)
                    {
                        _tournamentViewModel.Draws.Add(team1);
                    }

                    // Team2
                    if (team2.IsWinner == true)
                    {
                        _tournamentViewModel.Winners.Add(team2);
                    }

                    if (team2.IsLoser == true)
                    {
                        _tournamentViewModel.Losers.Add(team2);
                    }

                    if (team2.IsDraw == true)
                    {
                        _tournamentViewModel.Draws.Add(team2);
                    }
                }
            }
            else
            {
                //Winners bracket
                foreach(var teamPairing in _tournamentViewModel.WinnersBracket)
                {
                    var team1 = teamPairing.Team1;
                    var team2 = teamPairing.Team2;

                    //Team1
                    if (team1.IsWinner == true)
                    {
                        _tournamentViewModel.Winners.Add(team1);
                    }

                    if (team1.IsLoser == true)
                    {
                        _tournamentViewModel.Losers.Add(team1);
                    }

                    if (team1.IsDraw == true)
                    {
                        _tournamentViewModel.Draws.Add(team1);
                    }

                    // Team2
                    if (team2.IsWinner == true)
                    {
                        _tournamentViewModel.Winners.Add(team2);
                    }

                    if (team2.IsLoser == true)
                    {
                        _tournamentViewModel.Losers.Add(team2);
                    }

                    if (team2.IsDraw == true)
                    {
                        _tournamentViewModel.Draws.Add(team2);
                    }
                }

                //Winners bracket
                foreach (var teamPairing in _tournamentViewModel.LosersBracket)
                {
                    var team1 = teamPairing.Team1;
                    var team2 = teamPairing.Team2;

                    //Team1
                    if (team1.IsWinner == true)
                    {
                        _tournamentViewModel.Winners.Add(team1);
                    }

                    if (team1.IsLoser == true)
                    {
                        _tournamentViewModel.Losers.Add(team1);
                    }

                    if (team1.IsDraw == true)
                    {
                        _tournamentViewModel.Draws.Add(team1);
                    }

                    // Team2
                    if (team2.IsWinner == true)
                    {
                        _tournamentViewModel.Winners.Add(team2);
                    }

                    if (team2.IsLoser == true)
                    {
                        _tournamentViewModel.Losers.Add(team2);
                    }

                    if (team2.IsDraw == true)
                    {
                        _tournamentViewModel.Draws.Add(team2);
                    }
                }
            }
                
            _tournamentViewModel.IsRoundFinished = true;
        }

        private void NextRound_Click(object sender, RoutedEventArgs e)
        {
            if(!_tournamentViewModel.IsRoundFinished)
            {
                MessageBox.Show("Nije moguće pokrenuti sljedeću rundu dok trenutna još traje.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Clear winners from last round
            _tournamentViewModel.WinnersBracket.Clear();

            // Matchmake winners with other winners or teams who had a draw
            _tournamentViewModel.MatchmakeTeams(_tournamentViewModel.Winners, _tournamentViewModel.Draws, _tournamentViewModel.NewlyAddedTeams, true);

            // Clear losers from last round
            _tournamentViewModel.LosersBracket.Clear();

            // Matchmake losers with other losers or teams who had a draw
            _tournamentViewModel.MatchmakeTeams(_tournamentViewModel.Losers, _tournamentViewModel.Draws, _tournamentViewModel.NewlyAddedTeams, false);

            _tournamentViewModel.Winners.Clear();
            _tournamentViewModel.Losers.Clear();

            //_tournamentViewModel.IsViewingWinnersBracket = true;
            _tournamentViewModel.IsRoundFinished = false;
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
                RoundDoneButton.Visibility = Visibility.Collapsed;
                FirstRoundPairings.Visibility = Visibility.Collapsed;
                WinnersBracketPairings.Visibility = Visibility.Collapsed;
                LosersBracketPairings.Visibility = Visibility.Collapsed;
            }
            else
            {
                StartTournamentButton.Visibility = Visibility.Collapsed;
            }

            if(_tournamentViewModel.RoundCount == 1)
            {
                FirstRoundPairings.Visibility = Visibility.Visible;
                NextRoundButton.Visibility = Visibility.Visible;
                RoundDoneButton.Visibility = Visibility.Visible;
                WinnersBracketPairings.Visibility = Visibility.Collapsed;
                LosersBracketPairings.Visibility = Visibility.Collapsed;
            }
            else
            {
                FirstRoundPairings.Visibility = Visibility.Collapsed;
            }

            if(_tournamentViewModel.RoundCount > 1)
            {
                if (_tournamentViewModel.IsViewingWinnersBracket)
                {
                    WinnersBracketPairings.Visibility = Visibility.Visible;
                    LosersBracketPairings.Visibility = Visibility.Collapsed;
                }
                else
                {
                    WinnersBracketPairings.Visibility = Visibility.Collapsed;
                    LosersBracketPairings.Visibility = Visibility.Visible;
                }

                NextRoundButton.Visibility = Visibility.Visible;
                RoundDoneButton.Visibility = Visibility.Visible;

            }
        }

    }
}
