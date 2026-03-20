using System.ComponentModel;
using TournamentManager.HelperClasses;

namespace TournamentManager.ViewModels
{
    public class RoundHistoryViewModel : INotifyPropertyChanged
    {
        // Used to make a list of clickable elements in RoundHistory page
        private List<int> _rounds = new List<int>();
        public List<int> Rounds
        {
            get { return _rounds; }
            set { _rounds = value; }
        }

        private int? _selectedRound;
        public int? SelectedRound
        {
            get { return _selectedRound; }
            set { _selectedRound = value; OnPropertyChanged(nameof(SelectedRound)); }
        }

        // History of played rounds
        private List<RoundHistoryInfo> _allRounds = new List<RoundHistoryInfo>();
        public List<RoundHistoryInfo> AllRounds
        {
            get { return _allRounds; }
            set { _allRounds = value; }
        }

        private RoundHistoryInfo _roundToView;
        public RoundHistoryInfo RoundToView
        {
            get { return _roundToView; }
            set { _roundToView = value; OnPropertyChanged(nameof(RoundToView)); }
        }

        public void SetRoundToView(int roundCount)
        {
            SelectedRound = roundCount;
            
            var roundInfo = AllRounds.Where(x => x.Round == SelectedRound).FirstOrDefault();
            if (roundInfo != null) 
            {
                RoundToView = roundInfo;
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
