using System.Collections.ObjectModel;

namespace TournamentManager.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public ObservableCollection<Office> Offices { get; set; } = new ObservableCollection<Office>();
    }
}
