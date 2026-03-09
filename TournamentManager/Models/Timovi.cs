using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager.Models
{
    public class Timovi
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public int CountryId { get; set; }
    }
}
