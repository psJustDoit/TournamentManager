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
    /// Interaction logic for EditPoslovnica.xaml
    /// </summary>
    public partial class EditOfficeModal : Window
    {
        public int? officeId;

        public string? officeName { get; set; }
        public string? address { get; set; }
        public int? selectedCountryId;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Country> Countries { get; set; } = new ObservableCollection<Country>();
        
        public EditOfficeModal(Office office)
        {
            LoadCountries();
            OfficeId = office?.Id;
            OfficeName = office?.Name;
            Address = office?.Address;
            selectedCountryId = office?.CountryId;

            InitializeComponent();
            DataContext = this;
        }

        public int? SelectedCountryId
        {
            get => selectedCountryId.Value;
            set
            {
                if (selectedCountryId != value)
                {
                    selectedCountryId = value;
                    OnPropertyChanged(nameof(SelectedCountryId));

                }
            }
        }

        public string? Address
        {
            get => address;
            set
            {
                if (address != value)
                {
                    address = value;
                    OnPropertyChanged(nameof(address));

                }
            }
        }

        public string? OfficeName
        {
            get => officeName;
            set
            {
                if (officeName != value)
                {
                    officeName = value;
                    OnPropertyChanged(nameof(officeName));

                }
            }
        }

        public int? OfficeId
        {
            get => officeId;
            set
            {
                if (officeId != value)
                {
                    officeId = value;
                    OnPropertyChanged(nameof(officeId));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadCountries()
        {
            Countries = new ObservableCollection<Country>(DbRepository.GetCountries());
        }

        private void EditPoslovnicu_Click(object sender, RoutedEventArgs e)
        {
            int result = 0;
            try
            {
                result = DbRepository.EditOffice(new Office
                { Id = OfficeId.Value, 
                    Name = OfficeName, 
                    Address = Address, 
                    CountryId = SelectedCountryId.Value }
                );
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if(result != 0)
            {
                MessageBox.Show("Poslovnica ažurirana", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DialogResult = true;
        }
    }
}
