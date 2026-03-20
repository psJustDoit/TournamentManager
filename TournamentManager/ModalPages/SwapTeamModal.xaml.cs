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
        public uint? SelectedTeamId { get; set; }
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
                    possibleTeamSwaps = tournamentViewModel.AllTeams.Where(t => t.TeamId != teamToSwap.TeamId && t.TeamId != _pairing.Team2.TeamId && !pairing.Team2.TeamsIdsAlreadyPlayedWith.Contains(t.TeamId)).ToList(); 
                    break;
                case TeamEnum.Team2:
                    possibleTeamSwaps = tournamentViewModel.AllTeams.Where(t => t.TeamId != teamToSwap.TeamId && t.TeamId != _pairing.Team1.TeamId && !pairing.Team1.TeamsIdsAlreadyPlayedWith.Contains(t.TeamId)).ToList();
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
                SelectedTeamId = (uint)selectedTeam.TeamId;
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedTeamId == null)
            {
                MessageBox.Show("Nije selektiran tim za zamijenu", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var teamPairingOfSelectedTeam = _tournamentViewModel.TeamPairings.Where(tp => tp.Team1.TeamId == SelectedTeamId || tp.Team2.TeamId == SelectedTeamId).FirstOrDefault();

            Team? selectedTeamOpponent = null;
            if (teamPairingOfSelectedTeam != null) 
            {
                if (teamPairingOfSelectedTeam.Team1.TeamId == SelectedTeamId) 
                {
                    selectedTeamOpponent = teamPairingOfSelectedTeam.Team2;
                }
                else
                {
                    selectedTeamOpponent = teamPairingOfSelectedTeam.Team1;
                }
            }

            if(selectedTeamOpponent != null || !selectedTeamOpponent.IsDummyTeam)
            {
                if (selectedTeamOpponent.TeamsIdsAlreadyPlayedWith.Contains(_teamToSwap.TeamId))
                {
                    MessageBox.Show("Protivnik odabranog tima je već igrao protiv tima s kojim se želi napraviti zamijena", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Swap teams
            Team teamCopy = null;
            if(teamPairingOfSelectedTeam.Team1.TeamId == SelectedTeamId)
            {
                teamCopy = teamPairingOfSelectedTeam.Team1;
                teamPairingOfSelectedTeam.Team1 = _teamToSwap;
                teamPairingOfSelectedTeam.Team2.Opponent = _teamToSwap;
                _teamToSwap.Opponent = teamPairingOfSelectedTeam.Team2;       
            }
            else
            {
                teamCopy = teamPairingOfSelectedTeam.Team2;
                teamPairingOfSelectedTeam.Team2 = _teamToSwap;
                teamPairingOfSelectedTeam.Team1.Opponent = _teamToSwap;
                _teamToSwap.Opponent = teamPairingOfSelectedTeam.Team1;
            }

            switch (_teamToSwapPosition)
            {
                case TeamEnum.Team1:
                    _pairing.Team1 = teamCopy;
                    _pairing.Team1.Opponent = _pairing.Team2;
                    _pairing.Team2.Opponent = _pairing.Team1;
                    break;
                case TeamEnum.Team2:
                    _pairing.Team2 = teamCopy;
                    _pairing.Team2.Opponent = _pairing.Team1;
                    _pairing.Team1.Opponent = _pairing.Team2;
                    break;
                default:
                    break;
            }

            this.Close();
        }
    }
}
