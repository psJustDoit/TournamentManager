using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager
{
    public class TournamentType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public TournamentType(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
