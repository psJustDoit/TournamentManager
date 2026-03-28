using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TournamentManager.HelperClasses;
using TournamentManager.Models;
using TournamentManager.ViewModels;
using TournamentManager.Enums;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for Tournament.xaml
    /// </summary>

    public partial class Tournament : Page
    {
        private readonly TournamentViewModel _tournamentViewModel;
        private readonly RoundHistoryViewModel _roundHistoryViewModel;
        public Tournament(TournamentViewModel tournamentViewModel, RoundHistoryViewModel roundHistoryViewModel)
        {
            _tournamentViewModel = tournamentViewModel;
            _roundHistoryViewModel = roundHistoryViewModel;

            InitializeComponent();

            DataContext = _tournamentViewModel;
            UpdateVisibility();
            _roundHistoryViewModel = roundHistoryViewModel;
        }


        private void StartTournament_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_tournamentViewModel.AllTeams.Any())
                {
                    MessageBox.Show("Nema dodanih timova.", "Greška", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (_tournamentViewModel.SelectedTournamentType == null)
                {
                    MessageBox.Show("Nije selektirana igra za turnir", "Greška", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if(String.IsNullOrEmpty(NumberOfRoundsTextbox.Text))
                {
                    MessageBox.Show("Nije unesen maksimalan broj rundi", "Greška", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var maxNumberOfRounds = Convert.ToInt32(NumberOfRoundsTextbox.Text.Trim());

                _tournamentViewModel.MatchmakeTeamsFirstRound();
                _tournamentViewModel.IncrementRound();
                _tournamentViewModel.RoundStartDateTime = DateTime.Now;
                _tournamentViewModel.TournamentState = TournamentState.Started;
                _tournamentViewModel.MaxNumberOfRounds = maxNumberOfRounds;

                UpdateVisibility();
            }
            catch(FormatException fex)
            {
                MessageBox.Show(fex.Message, "Error", MessageBoxButton.OK);
            }
            catch(OverflowException oex)
            {
                MessageBox.Show(oex.Message, "Error", MessageBoxButton.OK);
            }
            
        }

        private void RestartFirstRound_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Resetirati prvu rundu?", "Restart", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) 
            {
                _tournamentViewModel.RestartFirstRound();
                UpdateVisibility();
            }
        }

        private void FinishTournament_Click(object sender, EventArgs e)
        {
            if (_tournamentViewModel.HasAnyUnresolvedPairs())
            {
                MessageBox.Show("Postoje parovi gdje rezultat još nije odlučen", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _tournamentViewModel.HandleAllTeamsWinsLossesDrawsAndScore();

            _tournamentViewModel.SortTeamsForScoreboard();

            _tournamentViewModel.TournamentState = TournamentState.Finished;

            _roundHistoryViewModel.CreateRoundHistoryEntry(_tournamentViewModel);

            MessageBox.Show("Turnir završen", "Kraj", MessageBoxButton.OK, MessageBoxImage.Information);

            FinishTournamentButton.Visibility = Visibility.Collapsed;
            TournamentFinishedText.Visibility = Visibility.Visible;

            //TODO: Add creating tournament history
        }

        private void NextRound_Click(object sender, RoutedEventArgs e)
        {
            var unresolvedPairOutcomes = _tournamentViewModel.TeamPairings.Where(tp => tp.Team1 != null && tp.Team2 != null)
                .Where(tp => tp.Team1.IsWinner == null && tp.Team1.IsLoser == null && tp.Team1.IsDraw == null)
                .Where(tp => tp.Team2.IsWinner == null && tp.Team2.IsLoser == null && tp.Team2.IsDraw == null)
                .ToList();

            if (unresolvedPairOutcomes.Any())
            {
                MessageBox.Show("Postoje parovi gdje rezultat još nije odlučen", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _tournamentViewModel.HandleAllTeamsWinsLossesDrawsAndScore();

            _tournamentViewModel.SortTeamsForScoreboard();

            _roundHistoryViewModel.CreateRoundHistoryEntry(_tournamentViewModel);

            _tournamentViewModel.TeamPairings.Clear();

            _tournamentViewModel.MatchmakeTeamsNext();

            _tournamentViewModel.IncrementRound();

            
            UpdateVisibility();
        }


        private void TeamMatchOutcome_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var matchOutcome = (TeamMatchOutcomeEnum)button.Tag;

            // walk up to find the root Border of the item
            var rootBorder = FindParent<Border>(button);

            // find the outcome borders by Tag
            var team1Border = FindChildByTag<Border>(rootBorder, "Team1Outcome");
            var team2Border = FindChildByTag<Border>(rootBorder, "Team2Outcome");

            var teamPairing = button.DataContext as TeamPairing;
            var team1 = teamPairing.Team1;
            var team2 = teamPairing.Team2;

            if (team1 == null || team2 == null)
            {
                return;
            }

            switch (matchOutcome)
            {
                case TeamMatchOutcomeEnum.Team1Win:
                    team1.IsWinner = true;
                    team1.IsDraw = null;
                    team1.IsLoser = null;

                    team2.IsWinner = null;
                    team2.IsDraw = null;
                    team2.IsLoser = true;

                    team1Border.Background = Brushes.Green;
                    team2Border.Background = Brushes.Red;
                    break;
                case TeamMatchOutcomeEnum.Team2Win:
                    team2.IsWinner = true;
                    team2.IsDraw = null;
                    team2.IsLoser = null;

                    team1.IsWinner = null;
                    team1.IsDraw = null;
                    team1.IsLoser = true;

                    team2Border.Background = Brushes.Green;
                    team1Border.Background = Brushes.Red;
                    break;
                case TeamMatchOutcomeEnum.Draw:
                    team1.IsWinner = null;
                    team1.IsDraw = true;
                    team1.IsLoser = null;

                    team2.IsWinner = null;
                    team2.IsDraw = true;
                    team2.IsLoser = null;

                    team1Border.Background = Brushes.Yellow;
                    team2Border.Background = Brushes.Yellow;
                    break;
            }
        }


        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent is T t) return t;
            return FindParent<T>(parent);
        }


        private T FindChildByTag<T>(DependencyObject parent, string tag = null) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T fe && fe.Tag as string == tag)
                {
                    return fe;
                }

                var result = FindChildByTag<T>(child, tag);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private void EditScoreDifference_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var rootBorder = FindParent<Border>(button);
            var teamPairing = rootBorder.DataContext as TeamPairing;

            var modal = new EditScoreDifferenceModal(teamPairing);
            modal.Owner = Window.GetWindow(this);

            bool? result = modal.ShowDialog();
        }

        private void UpdateVisibility()
        {      
            if(_tournamentViewModel.TournamentState == TournamentState.Started)
            {
                StartTournamentButton.Visibility = Visibility.Collapsed;
                NextRoundButton.Visibility = Visibility.Visible;
                TournamentTypeSelect.IsEnabled = false;
                NumberOfRoundsTextbox.IsEnabled = false;
                Scoreboard.Visibility = Visibility.Visible;

                if (_tournamentViewModel.RoundCount == _tournamentViewModel.MaxNumberOfRounds)
                {
                    NextRoundButton.Visibility = Visibility.Collapsed;
                    FinishTournamentButton.Visibility = Visibility.Visible;
                }

                if(_tournamentViewModel.RoundCount == 1)
                {
                    RestartFirstRoundButton.Visibility = Visibility.Visible;
                }
                else
                {
                    RestartFirstRoundButton.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                StartTournamentButton.Visibility = Visibility.Visible;
                NextRoundButton.Visibility = Visibility.Collapsed;
                FinishTournamentButton.Visibility = Visibility.Collapsed;
                TournamentTypeSelect.IsEnabled = true;
                NumberOfRoundsTextbox.IsEnabled = true;
                Scoreboard.Visibility = Visibility.Collapsed;
                RestartFirstRoundButton.Visibility = Visibility.Collapsed;
                TournamentFinishedText.Visibility = Visibility.Collapsed;
            }
        }


        private void TeamSwap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var rootBorder = FindParent<Border>(button);
                var teamPairing = rootBorder.DataContext as TeamPairing;
                var teamPositionToSwap = (TeamEnum)button.Tag;
                Team? teamToSwap = null;

                switch (teamPositionToSwap)
                {
                    case TeamEnum.Team1:
                        teamToSwap = teamPairing.Team1;
                        break;
                    case TeamEnum.Team2:
                        teamToSwap = teamPairing.Team2;
                        break;
                }

                var modal = new SwapTeamModal(teamPairing, _tournamentViewModel, teamToSwap, teamPositionToSwap);
                modal.Owner = Window.GetWindow(this);

                bool? result = modal.ShowDialog();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                //TODO: Add logging
            }
        }

        //TODO: Rework with having additional lists
        private void TeamKick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var rootBorder = FindParent<Border>(button);
                var teamPairing = rootBorder.DataContext as TeamPairing;
                var teamToKickPosition = (TeamEnum)button.Tag;

                Team? teamToKick = null;

                switch (teamToKickPosition)
                {
                    case TeamEnum.Team1:
                        teamToKick = teamPairing?.Team1;
                        break;
                    case TeamEnum.Team2:
                        teamToKick = teamPairing?.Team2;
                        break;
                }

                if(teamToKick == null || teamToKick.IsDummyTeam == true)
                {
                    return;
                }

                var result = MessageBox.Show($"Izbaciti tim {teamToKick.Name}?", "Izbaci", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    switch (teamToKickPosition)
                    {
                        case TeamEnum.Team1:
                            teamPairing.Team1 = null;
                            break;
                        case TeamEnum.Team2:
                            teamPairing.Team2 = null;
                            break;
                    }

                    teamToKick.IsKicked = true;

                    if(teamPairing.Team1 == null && teamPairing.Team2 == null)
                    {
                        _tournamentViewModel.TeamPairings.Remove(teamPairing);
                    }

                    //_tournamentViewModel.AllTeams.Remove(teamToKick);
                    _tournamentViewModel.SortTeamsForScoreboard();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
                //TODO: Add logging
            }

        }

        private void TournamentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (sender as ComboBox).SelectedItem as TournamentType;

            if (selected != null)
            {
                _tournamentViewModel.SelectedTournamentType = selected;
            }
        }

    }
}
