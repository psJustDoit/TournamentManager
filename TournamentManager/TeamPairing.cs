using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class TeamPairing
    {
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }

        public TeamPairing(Team team1, Team team2) 
        {
            Team1 = team1;
            Team2 = team2;
        }
    }
}
