using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using TournamentManager.ViewModels;
using TournamentManager.HelperClasses;


namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for RoundHistory.xaml
    /// </summary>
    public partial class RoundHistory : Page, INotifyPropertyChanged
    {
        private readonly RoundHistoryViewModel _roundHistoryViewModel;

        public RoundHistory(RoundHistoryViewModel roundHistoryViewModel)
        {
            _roundHistoryViewModel = roundHistoryViewModel;

            InitializeComponent();

            DataContext = _roundHistoryViewModel;
        }

        public ICommand RoundToViewCommand { get; }

        private void RoundToView_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var roundHistoryInfo = border.DataContext as RoundHistoryInfo;

            if(roundHistoryInfo.Round != _roundHistoryViewModel.SelectedRound)
            {
                _roundHistoryViewModel.SetRoundToView(roundHistoryInfo.Round);
            }
        }

        private void SelectedRound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (int?)(sender as ComboBox).SelectedItem;

            if (selected != null)
            {
                _roundHistoryViewModel.SetRoundToView(selected.Value);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
