using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for Poslovnice.xaml
    /// </summary>
    public partial class Poslovnice : Page, INotifyPropertyChanged
    {
        public ObservableCollection<DrzaveViewModel> drzaveToDisplay { get; set; } = new ObservableCollection<DrzaveViewModel>();

        public ObservableCollection<DrzaveViewModel> DrzaveToDisplay
        {
            get => drzaveToDisplay;
            set { drzaveToDisplay = value; OnPropertyChanged(nameof(DrzaveToDisplay)); }
        }

        public Poslovnice()
        {
            InitializeComponent();

            DataContext = this;

            LoadData();
        }

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler? PropertyChanged;

        public void LoadData()
        {
            DrzaveToDisplay = new ObservableCollection<DrzaveViewModel>(DbRepository.GetCountriesWithExistingOffices());
        }

        public void DodajPoslovnicu_Click(object sender, RoutedEventArgs e)
        {
            // Create modal window to add office
            var modal = new DodajPoslovnicuModal();

            modal.Owner = Window.GetWindow(this);

            bool? result = modal.ShowDialog();

            if (result == true)
            {
                // Reload data
                LoadData();
            }
        }

        private void EditOffice_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var office = menuItem.DataContext as PoslovniceViewModel;

            if (office == null) return;

            var modal = new EditPoslovnicaModal(office);

            modal.Owner = Window.GetWindow(this);

            bool? result = modal.ShowDialog();

            LoadData();
        }

        private void DeleteOffice_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var office = menuItem.DataContext as PoslovniceViewModel;

            if (office == null) return;

            var windowResult = MessageBox.Show($"Izbrisati poslovnicu {office.Name}?", "Delete", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);

            switch (windowResult)
            {
                case MessageBoxResult.OK:
                    int queryResult = DbRepository.DeleteOffice(office.Id);
                    if(queryResult != 0)
                    {
                        MessageBox.Show("Poslovnica uspješno izbrisana", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    break;
                default:
                    break;
            }

            LoadData();
        }
    }
}
