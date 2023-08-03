namespace MdNotesServer.Infrastructure.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Loging { get; set; }

        public List<Note> Notes { get; set; }
    }
}
