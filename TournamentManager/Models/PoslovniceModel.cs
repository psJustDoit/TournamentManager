using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager.Models
{
    public class PoslovniceModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int CountryId { get; set; }
        public string? Address { get; set; }
    }
}
