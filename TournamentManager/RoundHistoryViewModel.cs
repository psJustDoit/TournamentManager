using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TournamentManager
{
    public class RoundHistoryViewModel : INotifyPropertyChanged
    {
        // Used to make a list of clickable elements in RoundHistory page
        private List<uint> _rounds = new List<uint>();
        public List<uint> Rounds
        {
            get { return _rounds; }
            set { _rounds = value; }
        }

        private uint? _selectedRound;
        public uint? SelectedRound
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

        public void SetRoundToView(uint roundCount)
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
