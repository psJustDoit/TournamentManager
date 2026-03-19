using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class RoundHistoryInfo
    {
        public uint Round { get; set; }
        public List<TeamScore> Scoreboard {  get; set; }
        public List<TeamPairing> TeamPairing { get; set; }

        public DateTime? DateTimeEnded { get; set; }
    }
}
