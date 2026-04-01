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
using System.Windows.Shapes;
using TournamentManager.Db;
using TournamentManager.Models;
using TournamentManager.ViewModels;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for AddOfficeModal.xaml
    /// </summary>
    public partial class AddOfficeModal : Window, INotifyPropertyChanged
    {
        public string? OfficeName { get; set; }
        public string? Address { get; set; }

        private ObservableCollection<Country> _countries = new ObservableCollection<Country>();
        public ObservableCollection<Country> Countries
        {
            get => _countries;
            set { _countries = value; OnPropertyChanged(nameof(Countries)); }
        }

        public AddOfficeModal()
        {
            InitializeComponent();
            DataContext = this;
            LoadCountries();
        }

        public void LoadCountries()
        {
            Countries = new ObservableCollection<Country>(DbRepository.GetCountries());
        }

        private void DodajPoslovnicu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(OfficeName))
                {
                    MessageBox.Show("Poslovnica je prazna", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedCountry = (Country)CountriesCombobox.SelectedItem;
                if (selectedCountry == null)
                {
                    MessageBox.Show("Država nije odabrana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = DbRepository.AddOffice(OfficeName, Address, selectedCountry.CountryId);

                DialogResult = true;
                //if(result != 0)
                //{
                //    MessageBox.Show("Poslovnica dodana", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                //    DialogResult = true; // closes modal and returns true
                //}
            }
            catch
            {
                MessageBox.Show("Greška pri dodavanju poslovnice", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
