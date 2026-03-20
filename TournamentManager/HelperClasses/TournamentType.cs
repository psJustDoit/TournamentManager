namespace TournamentManager.HelperClasses
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
