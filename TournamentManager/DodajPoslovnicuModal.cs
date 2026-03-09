using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using TournamentManager.ViewModels;

namespace TournamentManager
{
    public partial class DodajPoslovnicuModal : Window, INotifyPropertyChanged
    {
        public string?  OfficeName { get; set; }
        public string? Address { get; set; }
        public int? selectedCountryId;

        public event PropertyChangedEventHandler? PropertyChanged;

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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<DrzaveViewModel> Countries { get; set; } = new ObservableCollection<DrzaveViewModel>();

        public DodajPoslovnicuModal()
        {
            InitializeComponent();
            DataContext = this;
            LoadCountries();
        }

        public void LoadCountries()
        {
            Countries = new ObservableCollection<DrzaveViewModel>(DbRepository.GetCountries());
        }

        private void DodajPoslovnicu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OfficeName == null || OfficeName == "")
                {
                    MessageBox.Show("Poslovnica je prazna", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedCountry = (DrzaveViewModel)CountriesCombobox.SelectedItem;
                if (selectedCountry == null)
                {
                    MessageBox.Show("Država nije odabrana", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                DbRepository.AddOffice(OfficeName, Address, selectedCountry.CountryId);
                MessageBox.Show("Poslovnica dodana", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true; // closes modal and returns true
            }
            catch
            {
                MessageBox.Show("Greška pri dodavanju poslovnice", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
