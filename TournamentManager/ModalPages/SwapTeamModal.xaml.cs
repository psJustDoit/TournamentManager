using System.Windows;
using System.Windows.Controls;
using TournamentManager.ViewModels;
using TournamentManager.Models;
using TournamentManager.HelperClasses;
using TournamentManager.Enums;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for SwapTeam.xaml
    /// </summary>
    public partial class SwapTeamModal : Window
    {
        private readonly TeamPairing _pairing;
        private readonly TournamentViewModel _tournamentViewModel;
        private Team _teamToSwap;
        public Team TeamToSwap
        {
            get => _teamToSwap;
        }

        private readonly TeamEnum _teamToSwapPosition;
        public int? SelectedTeamId { get; set; }
        public SwapTeamModal(TeamPairing pairing, TournamentViewModel tournamentViewModel, Team teamToSwap, TeamEnum teamToSwapPosition)
        {
            List<Team> possibleTeamSwaps = new List<Team>();

            _pairing = pairing;
            _tournamentViewModel = tournamentViewModel;
            _teamToSwap = teamToSwap;
            _teamToSwapPosition = teamToSwapPosition;

            switch (teamToSwapPosition)
            {
                case TeamEnum.Team1:
                    possibleTeamSwaps = tournamentViewModel.AllTeams.Where(t => t.TeamId != teamToSwap.TeamId)
                        .Where(t => t.TeamId != _pairing.Team2?.TeamId)
                        .Where(t => t.IsKicked != true)
                        .Where(t => pairing.Team2?.TeamsIdsAlreadyPlayedWith.Contains(t.TeamId) == false)
                        .ToList();
                   
                    break;
                case TeamEnum.Team2:
                    possibleTeamSwaps = tournamentViewModel.AllTeams.Where(t => t.TeamId != teamToSwap.TeamId)
                        .Where(t => t.TeamId != _pairing.Team1?.TeamId)
                        .Where(t => t.IsKicked != true)
                        .Where(t => pairing.Team1?.TeamsIdsAlreadyPlayedWith.Contains(t.TeamId) == false)
                        .ToList();
                    break;
                default:
                    break;
            }

            InitializeComponent();
            
            DataContext = possibleTeamSwaps;
        }

        private void Team_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTeam = (sender as ComboBox).SelectedItem as Team;

            if (selectedTeam != null)
            {
                SelectedTeamId = selectedTeam.TeamId;
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedTeamId == null)
            {
                MessageBox.Show("Nije selektiran tim za zamijenu", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var teamPairingOfSelectedTeam = _tournamentViewModel.TeamPairings.Where(tp => tp.Team1?.TeamId == SelectedTeamId || tp.Team2?.TeamId == SelectedTeamId).FirstOrDefault();

            Team? selectedTeamOpponent = null;
            if (teamPairingOfSelectedTeam != null) 
            {
                if (teamPairingOfSelectedTeam.Team1?.TeamId == SelectedTeamId) 
                {
                    selectedTeamOpponent = teamPairingOfSelectedTeam.Team2;
                }
                else
                {
                    selectedTeamOpponent = teamPairingOfSelectedTeam.Team1;
                }
            }
            else
            {
                MessageBox.Show("Nije pronađen par odabranog tima", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedTeamOpponent != null && !selectedTeamOpponent.IsDummyTeam)
            {
                if (selectedTeamOpponent.TeamsIdsAlreadyPlayedWith.Contains(_teamToSwap.TeamId))
                {
                    MessageBox.Show("Protivnik odabranog tima je već igrao protiv tima s kojim se želi napraviti zamijena", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Swap teams
            Team teamCopy = null;
            if(teamPairingOfSelectedTeam.Team1?.TeamId == SelectedTeamId)
            {
                teamCopy = teamPairingOfSelectedTeam.Team1; // Get selected team from its team pairing
                teamPairingOfSelectedTeam.Team1 = _teamToSwap; // Place team to be swapped in place of selected team
                teamPairingOfSelectedTeam.Team2?.Opponent = _teamToSwap; // Set the other teams opponent to now swapped team
                _teamToSwap.Opponent = teamPairingOfSelectedTeam.Team2; // Update now swapped teams opponent
            }
            else
            {
                teamCopy = teamPairingOfSelectedTeam.Team2;
                teamPairingOfSelectedTeam.Team2 = _teamToSwap;
                teamPairingOfSelectedTeam.Team1?.Opponent = _teamToSwap;
                _teamToSwap.Opponent = teamPairingOfSelectedTeam.Team1;
            }

            // Update the pairing of team to be swapped with selected team
            switch (_teamToSwapPosition)
            {
                case TeamEnum.Team1:
                    _pairing.Team1 = teamCopy;
                    _pairing.Team1?.Opponent = _pairing.Team2;
                    _pairing.Team2?.Opponent = _pairing.Team1;
                    break;
                case TeamEnum.Team2:
                    _pairing.Team2 = teamCopy;
                    _pairing.Team2?.Opponent = _pairing.Team1;
                    _pairing.Team1?.Opponent = _pairing.Team2;
                    break;
                default:
                    break;
            }

            this.Close();
        }
    }
}
