namespace TournamentManager.Models
{
    public class Office
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public int CountryId {  get; set; }
    }
}
