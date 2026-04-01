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

                if(String.IsNullOrEmpty(NumberOfRoundsTextbox.Text) || NumberOfRoundsTextbox.Text.Trim() == "0")
                {
                    MessageBox.Show("Nije unesen maksimalan broj rundi", "Greška", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var maxNumberOfRounds = Convert.ToInt32(NumberOfRoundsTextbox.Text.Trim());

                _tournamentViewModel.MatchmakeTeamsFirstRound();
                _tournamentViewModel.IncrementRound();
                _tournamentViewModel.TournamentStartDateTime = DateTime.Now;
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


        private void ResetRound_Click(object sender, EventArgs e)
        {
            if (_tournamentViewModel.TournamentState == TournamentState.NotStarted || _tournamentViewModel.TournamentState == TournamentState.Finished)
            {
                MessageBox.Show("Nije moguće resetirati rundu", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Resetirati rundu? Ovo će iznova upariti timove.", "Runda - reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _tournamentViewModel.ResetRound();
            }
        }


        private void RestartTournament_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Resetirati turnir?", "Turir - reset", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes) 
            {
                //var resultResetTeams = MessageBox.Show("Resetirati timove?", "Restart timove", MessageBoxButton.YesNo, MessageBoxImage.Question);

                //bool resetTeams = resultResetTeams == MessageBoxResult.Yes ? true : false;

                _tournamentViewModel.RestartTournament();
                _roundHistoryViewModel.RestartRoundsHistory();
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

            _tournamentViewModel.HandleAllTeamsWinsLossesDrawsAndScores();

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
            if (_tournamentViewModel.HasAnyUnresolvedPairs())
            {
                MessageBox.Show("Postoje parovi gdje rezultat još nije odlučen", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _tournamentViewModel.HandleAllTeamsWinsLossesDrawsAndScores();

            _tournamentViewModel.SortTeamsForScoreboard();

            _roundHistoryViewModel.CreateRoundHistoryEntry(_tournamentViewModel);

            _tournamentViewModel.TeamPairings.Clear();

            _tournamentViewModel.MatchmakeTeams();

            _tournamentViewModel.IncrementRound();

            
            UpdateVisibility();
        }


        private void TeamMatchOutcome_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var matchOutcome = (TeamPairingMatchOutcomeEnum)button.Tag;

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
                case TeamPairingMatchOutcomeEnum.Team1Win:
                    team1.TeamMatchOutcomeCurrent = TeamMatchOutcomeEnum.Winner;
                    team2.TeamMatchOutcomeCurrent = TeamMatchOutcomeEnum.Loser;

                    team1Border.Background = Brushes.Green;
                    team2Border.Background = Brushes.Red;
                    break;
                case TeamPairingMatchOutcomeEnum.Team2Win:
                    team2.TeamMatchOutcomeCurrent = TeamMatchOutcomeEnum.Winner;
                    team1.TeamMatchOutcomeCurrent = TeamMatchOutcomeEnum.Loser;

                    team2Border.Background = Brushes.Green;
                    team1Border.Background = Brushes.Red;
                    break;
                case TeamPairingMatchOutcomeEnum.Draw:
                    team1.TeamMatchOutcomeCurrent = TeamMatchOutcomeEnum.Draw;
                    team2.TeamMatchOutcomeCurrent = TeamMatchOutcomeEnum.Draw;

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
            if (_tournamentViewModel.TournamentState == TournamentState.NotStarted)
            {
                TournamentNotStartedVisibility();
            }

            if (_tournamentViewModel.TournamentState == TournamentState.Started)
            {
                TournamentStartedVisibility();
            }

            if (_tournamentViewModel.TournamentState == TournamentState.Finished)
            {
                TournamentFinishedVisibility();
            }

        }

        private void TournamentStartedVisibility()
        {
            StartTournamentButton.Visibility = Visibility.Collapsed;

            if(_tournamentViewModel.RoundCount == _tournamentViewModel.MaxNumberOfRounds)
            {
                NextRoundButton.Visibility = Visibility.Collapsed;
                FinishTournamentButton.Visibility = Visibility.Visible;
            }
            else
            {
                NextRoundButton.Visibility = Visibility.Visible;
                FinishTournamentButton.Visibility = Visibility.Collapsed;
            }

            TournamentFinishedText.Visibility = Visibility.Collapsed;
            TournamentTypeSelect.IsEnabled = false;
            NumberOfRoundsTextbox.IsEnabled = false;
            Scoreboard.Visibility = Visibility.Visible;
        }

        private void TournamentFinishedVisibility()
        {
            TournamentFinishedText.Visibility = Visibility.Visible;
            FinishTournamentButton.Visibility = Visibility.Collapsed;
            StartTournamentButton.Visibility = Visibility.Collapsed;
            NextRoundButton.Visibility = Visibility.Collapsed;
            TournamentTypeSelect.IsEnabled = false;
            NumberOfRoundsTextbox.IsEnabled = false;
            Scoreboard.Visibility = Visibility.Visible;
        }

        private void TournamentNotStartedVisibility()
        {
            StartTournamentButton.Visibility = Visibility.Visible;
            NextRoundButton.Visibility = Visibility.Collapsed;
            FinishTournamentButton.Visibility = Visibility.Collapsed;
            TournamentFinishedText.Visibility = Visibility.Collapsed;
            TournamentTypeSelect.IsEnabled = true;
            NumberOfRoundsTextbox.IsEnabled = true;
            Scoreboard.Visibility = Visibility.Collapsed;
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
