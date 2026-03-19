using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TournamentManager.ViewModels;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for Timovi.xaml
    /// </summary>
    public partial class Timovi : Page
    {
        private readonly TournamentViewModel _tournamentViewModel;

        public Timovi(TournamentViewModel tournamentViewModel)
        {
            _tournamentViewModel = tournamentViewModel;

            InitializeComponent();

            DataContext = _tournamentViewModel;
        }

        private void AddTeam_Click(object sender, RoutedEventArgs e) 
        {
            // Create modal window to add office
            var modal = new DodajTimModal(_tournamentViewModel);

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

            var modal = new AzurirajTimModal(teamToUpdate);
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
