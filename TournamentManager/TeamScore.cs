using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class TeamScore
    {
        public string Name { get; set; }
        public int TournamentScore { get; set; }
        public int? ScoreDifference { get; set; }
    }
}
