using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager.ViewModels
{
    public class DrzaveViewModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public ObservableCollection<PoslovniceViewModel> Poslovnice { get; set; } = new ObservableCollection<PoslovniceViewModel>();
    }
}
