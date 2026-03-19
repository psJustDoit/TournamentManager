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
        private readonly RoundHistoryViewModel _roundHistoryViewModel;
        public Turnir(TournamentViewModel tournamentViewModel, RoundHistoryViewModel roundHistoryViewModel)
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
            if (!_tournamentViewModel.AllTeams.Any())
            {
                MessageBox.Show("Nema dodanih timova.", "Greška", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if(_tournamentViewModel.SelectedTournamentType == null)
            {
                MessageBox.Show("Nije selektirana igra za turnir", "Greška", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _tournamentViewModel.MatchmakeTeamsFirstRound();
            _tournamentViewModel.IncrementRound();
            _tournamentViewModel.RoundStartDateTime = DateTime.Now;

            UpdateVisibility();
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

            foreach (var team in _tournamentViewModel.AllTeams)
            {
                if (team.Opponent != null)
                {
                    if (team.IsLoser == true)
                    {
                        team.IncreaseTeamLoss();
                    }

                    if (team.IsWinner == true)
                    {
                        team.TeamIdsWonAgainst.Add(team.Opponent.TeamId);
                        team.IncreaseTeamWin();
                    }

                    if (team.IsDraw == true)
                    {
                        team.IncreaseTeamDraw();
                    }

                    team.SetScoreDifference(team.Opponent);

                    if(team.Opponent.IsDummyTeam == false)
                    {
                        team.TeamsIdsAlreadyPlayedWith.Add(team.Opponent.TeamId);
                    }
                    

                    team.Opponent = null;
                }
            }

            foreach (var dummy in _tournamentViewModel.DummyTeams) 
            {
                dummy.Opponent = null;
            }

            CreateRoundHistoryEntry();

            _tournamentViewModel.TeamPairings.Clear();

            _tournamentViewModel.MatchmakeTeamsNext();
            _tournamentViewModel.IncrementRound();

            _tournamentViewModel.AllTeams = new ObservableCollection<Team>(_tournamentViewModel.AllTeams.OrderByDescending(x => x.TeamTournamentScore));
            UpdateVisibility();
        }

        private void CreateRoundHistoryEntry()
        {
            var roundHistoryInfo = new RoundHistoryInfo();
            var teamPairingsHistoryInfo = new List<TeamPairing>();
            foreach (var teamPairing in _tournamentViewModel.TeamPairings)
            {
                Team? team1 = null;
                if (teamPairing.Team1 != null)
                {
                    team1 = new Team(teamPairing.Team1.TeamId, teamPairing.Team1.Name, teamPairing.Team1.IsDummyTeam, null);
                    team1.IsDraw = teamPairing.Team1.IsDraw;
                    team1.IsLoser = teamPairing.Team1.IsLoser;
                    team1.IsWinner = teamPairing.Team1.IsWinner;
                    team1.TeamTournamentScore = teamPairing.Team1.TeamTournamentScore;
                    team1.GameMatchScore = teamPairing.Team1.GameMatchScore;
                    team1.ScoreDifference = teamPairing.Team1.ScoreDifference;
                    team1.Wins = teamPairing.Team1.Wins;
                    team1.Losses = teamPairing.Team1.Losses;
                    team1.Draws = teamPairing.Team1.Draws;
                    team1.TeamsIdsAlreadyPlayedWith = teamPairing.Team1.TeamsIdsAlreadyPlayedWith;
                    team1.TeamIdsWonAgainst = teamPairing.Team1.TeamIdsWonAgainst;
                }

                Team? team2 = null;
                if (teamPairing.Team2 != null) 
                {
                    team2 = new Team(teamPairing.Team2.TeamId, teamPairing.Team2.Name, teamPairing.Team2.IsDummyTeam, null);
                    team2.IsDraw = teamPairing.Team2.IsDraw;
                    team2.IsLoser = teamPairing.Team2.IsLoser;
                    team2.IsWinner = teamPairing.Team2.IsWinner;
                    team2.TeamTournamentScore = teamPairing.Team2.TeamTournamentScore;
                    team2.GameMatchScore = teamPairing.Team2.GameMatchScore;
                    team2.ScoreDifference = teamPairing.Team2.ScoreDifference;
                    team2.Wins = teamPairing.Team2.Wins;
                    team2.Losses = teamPairing.Team2.Losses;
                    team2.Draws = teamPairing.Team2.Draws;
                    team2.TeamsIdsAlreadyPlayedWith = teamPairing.Team2.TeamsIdsAlreadyPlayedWith;
                    team2.TeamIdsWonAgainst = teamPairing.Team2.TeamIdsWonAgainst;

                }

                team1?.Opponent = team2;
                team2?.Opponent = team1;

                var pairing = new TeamPairing(team1, team2);

                teamPairingsHistoryInfo.Add(pairing);
            }

            var currentScoreboard = _tournamentViewModel.AllTeams
                .OrderByDescending(t => t.TeamTournamentScore)
                .Select(t => new TeamScore
                {
                    Name = t.Name,
                    TournamentScore = t.TeamTournamentScore,
                    ScoreDifference = t.ScoreDifference
                }).ToList();

            roundHistoryInfo.Round = _tournamentViewModel.RoundCount;
            roundHistoryInfo.Scoreboard = currentScoreboard;
            roundHistoryInfo.TeamPairing = teamPairingsHistoryInfo;
            roundHistoryInfo.DateTimeEnded = DateTime.Now;

            // Add round info to round histories
            _roundHistoryViewModel.Rounds.Add(_tournamentViewModel.RoundCount);
            _roundHistoryViewModel.AllRounds.Add(roundHistoryInfo);
        }

        private void TeamMatchOutcome_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var matchOutcome = (TeamMatchOutcome)button.Tag;

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
                case TeamMatchOutcome.Team1Win:
                    team1.IsWinner = true;
                    team1.IsDraw = null;
                    team1.IsLoser = null;

                    team2.IsWinner = null;
                    team2.IsDraw = null;
                    team2.IsLoser = true;

                    team1Border.Background = Brushes.Green;
                    team2Border.Background = Brushes.Red;
                    break;
                case TeamMatchOutcome.Team2Win:
                    team2.IsWinner = true;
                    team2.IsDraw = null;
                    team2.IsLoser = null;

                    team1.IsWinner = null;
                    team1.IsDraw = null;
                    team1.IsLoser = true;

                    team2Border.Background = Brushes.Green;
                    team1Border.Background = Brushes.Red;
                    break;
                case TeamMatchOutcome.Draw:
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
            if (_tournamentViewModel.RoundCount == 0)
            {
                StartTournamentButton.Visibility = Visibility.Visible;
                NextRoundButton.Visibility = Visibility.Collapsed;
                TournamentTypeSelect.IsEnabled = true;
            }
            else
            {
                StartTournamentButton.Visibility = Visibility.Collapsed;
                NextRoundButton.Visibility = Visibility.Visible;
                TournamentTypeSelect.IsEnabled = false;
            }

            if(_tournamentViewModel.RoundCount > 1)
            {
                Scoreboard.Visibility = Visibility.Visible;
            }
            else
            {
                Scoreboard.Visibility = Visibility.Collapsed;
            }

        }

        //private void GetPossibleTeamSwaps_SubmenuOpened(object sender, RoutedEventArgs e)
        //{
        //    var menuItem = sender as MenuItem;
        //    var contextMenu = (sender as MenuItem)?.Parent as ContextMenu;
        //    var border = contextMenu?.PlacementTarget as Border;
        //    var tag = border?.Tag as string; // "Team1" or "Team2"
        //    var teamPairing = border?.DataContext as TeamPairing;

        //    var teamToSwap = tag == "Team1" ? teamPairing.Team1 : teamPairing.Team2;

        //    if (teamToSwap == null || teamToSwap.IsDummyTeam)
        //    {
        //        return;
        //    }

        //    if (teamToSwap != null)
        //    {
        //        menuItem.ItemsSource = null;
        //        menuItem.Items.Clear();
        //        var possibleTeamSwaps = GetPossibleTeamSwaps(menuItem, teamToSwap, teamPairing);
        //        menuItem.ItemsSource = possibleTeamSwaps;
        //    }
        //}

        //private List<MenuItem> GetPossibleTeamSwaps(MenuItem itemClicked, Team teamToSwap, TeamPairing teamPairing)
        //{
        //    int opponentTeamId = 0;
        //    if (teamPairing.Team1.TeamId == teamToSwap.TeamId)
        //    {
        //        opponentTeamId = teamPairing.Team2.TeamId;
        //    }
        //    else
        //    {
        //        opponentTeamId = teamPairing.Team1.TeamId;
        //    }

        //    var possibleTeamSwaps = _tournamentViewModel.AllTeams.Where(t => t.TeamId != teamToSwap.TeamId)
        //        .Where(t => !t.TeamsIdsAlreadyPlayedWith.Contains(teamToSwap.TeamId))
        //        .Where(t => t.TeamId != opponentTeamId)
        //        .Select(t => new MenuItem
        //        {
        //            Header = t.Name,
        //            Tag = t.TeamId
        //        }).ToList();

        //    foreach (var menuItem in possibleTeamSwaps)
        //    {
        //        menuItem.Click += (s, e) => SwapTeam(s, e, teamPairing, teamToSwap);
        //    }

        //    return possibleTeamSwaps;
        //}


        //private void SwapTeam(object sender, RoutedEventArgs e, TeamPairing teamPairing, Team teamToSwap)
        //{
        //    var item = sender as MenuItem;
        //    var teamPairingOfTeamSelectedToSwap = _tournamentViewModel.TeamPairings.Where(tp => tp.Team1.TeamId == (int)item.Tag || tp.Team2.TeamId == (int)item.Tag).FirstOrDefault();
        //    Team teamSelectedToSwap = null;

        //    if (teamPairingOfTeamSelectedToSwap.Team1.TeamId == (int)item.Tag)
        //    {
        //        teamSelectedToSwap = teamPairingOfTeamSelectedToSwap.Team1;
        //        teamPairingOfTeamSelectedToSwap.Team1 = teamToSwap;
        //    }
        //    else
        //    {
        //        teamSelectedToSwap = teamPairingOfTeamSelectedToSwap.Team2;
        //        teamPairingOfTeamSelectedToSwap.Team2 = teamToSwap;
        //    }

        //    if (teamPairing.Team1.TeamId == teamToSwap.TeamId)
        //    {
        //        teamPairing.Team1 = teamSelectedToSwap;
        //    }
        //    else
        //    {
        //        teamPairing.Team2 = teamSelectedToSwap;
        //    }
        //}

        //private void ResetSubmenu_SubmenuClosed(object sender, RoutedEventArgs e)
        //{
        //    var menuItem = sender as MenuItem;
        //    menuItem.ItemsSource = null;
        //    menuItem.Items.Clear();
        //    menuItem.Items.Add(new MenuItem { Header = "Loading..." });
        //}


        private void TeamSwap_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var rootBorder = FindParent<Border>(button);
            var teamPairing = rootBorder.DataContext as TeamPairing;
            var teamPositionToSwap = (TeamToSwapEnum)button.Tag;
            Team? teamToSwap = null;

            switch (teamPositionToSwap)
            {
                case TeamToSwapEnum.Team1:
                    teamToSwap = teamPairing.Team1;
                    break;
                case TeamToSwapEnum.Team2:
                    teamToSwap = teamPairing.Team2;
                    break;
            }

            var modal = new SwapTeamModal(teamPairing, _tournamentViewModel, teamToSwap, teamPositionToSwap);
            modal.Owner = Window.GetWindow(this);

            bool? result = modal.ShowDialog();
        }


        private void TeamKick_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var rootBorder = FindParent<Border>(button);
            var teamPairing = rootBorder.DataContext as TeamPairing;
            var teamToKickPosition = (TeamToKick)button.Tag;

            Team? teamToKick = null;

            switch (teamToKickPosition)
            {
                case TeamToKick.Team1:
                    teamToKick = teamPairing?.Team1;
                    break;
                case TeamToKick.Team2:
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
                foreach(var teamId in teamToKick.TeamIdsWonAgainst)
                {
                    var teamToAddPointTo = _tournamentViewModel.AllTeams.Where(t => t.TeamId == teamId).FirstOrDefault();
                    if(teamToAddPointTo != null)
                    {
                        teamToAddPointTo.TeamTournamentScore += 1;
                    }
                }

                
                if (teamToKickPosition == TeamToKick.Team1) 
                {
                    teamPairing.Team1 = null;
                    teamPairing.Team2.Opponent = null;
                }
                else
                {
                    teamPairing.Team2 = null;
                    teamPairing.Team1.Opponent = null;
                }

                _tournamentViewModel.AllTeams.Remove(teamToKick);
                _tournamentViewModel.SortTeamsByScoreDescending();
            }
            else
            {
                return;
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
