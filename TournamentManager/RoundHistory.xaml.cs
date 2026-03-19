using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using Wpf.Ui.Input;

namespace TournamentManager
{
    /// <summary>
    /// Interaction logic for RoundHistory.xaml
    /// </summary>
    public partial class RoundHistory : Page, INotifyPropertyChanged
    {
        //private ObservableCollection<int> _rounds;
        //public ObservableCollection<int> Rounds
        //{
        //    get { return _rounds; }
        //    set { _rounds = value; OnPropertyChanged(nameof(Rounds)); }
        //}


        //private ObservableCollection<RoundHistoryInfo> _roundHistories;
        //public ObservableCollection<RoundHistoryInfo> RoundHistories
        //{
        //    get { return _roundHistories; } 
        //    set { _roundHistories = value; OnPropertyChanged(nameof(RoundHistories));  }
        //}
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
            var selected = (uint?)(sender as ComboBox).SelectedItem;

            if (selected != null)
            {
                _roundHistoryViewModel.SetRoundToView(selected.Value);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
