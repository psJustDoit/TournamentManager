using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using TournamentManager.Models;
using TournamentManager.ViewModels;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for Teams.xaml
    /// </summary>
    public partial class Teams : Page
    {
        private readonly TournamentViewModel _tournamentViewModel;

        public Teams(TournamentViewModel tournamentViewModel)
        {
            _tournamentViewModel = tournamentViewModel;

            InitializeComponent();

            DataContext = _tournamentViewModel;
        }

        private void AddTeam_Click(object sender, RoutedEventArgs e) 
        {
            var modal = new AddTeamModal(_tournamentViewModel);

            modal.Owner = Window.GetWindow(this);

            modal.ShowDialog();
        }

        private void TeamUpdate_Click(Object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem.Parent as ContextMenu;
            var teamName = contextMenu.PlacementTarget as TextBlock;
            var teamToUpdate = teamName.DataContext as Team;

            if (teamToUpdate == null)
            {
                return;
            }

            var modal = new EditTeamModal(teamToUpdate, _tournamentViewModel);
            modal.Owner = Window.GetWindow(this);

            bool? result = modal.ShowDialog();

            if (result == true)
            {
                _tournamentViewModel.SortTeamsByScoreDescending();
            }
        }

        //private void DeleteTeam_Click(Object sender, RoutedEventArgs e)
        //{
        //    var menuItem = sender as MenuItem;
        //    var contextMenu = menuItem.Parent as ContextMenu;
        //    var button = contextMenu.PlacementTarget as Button;
        //    var teamToDelete = button.DataContext as Team;

        //    if(teamToDelete == null)
        //    {
        //        return;
        //    }

        //    var result = MessageBox.Show($"Izbrisati tim {teamToDelete.Name}?", "Delete", MessageBoxButton.YesNo);

        //    switch (result) 
        //    {
        //        case MessageBoxResult.Yes:
        //            _tournamentViewModel.AllTeams.Remove(teamToDelete);
        //            _tournamentViewModel.AllTeams.OrderByDescending(x => x.TeamTournamentScore);
        //            break;
        //        case MessageBoxResult.No:
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}
