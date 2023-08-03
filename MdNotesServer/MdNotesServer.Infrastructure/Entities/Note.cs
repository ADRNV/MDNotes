namespace MdNotesServer.Infrastructure.Entities
{
    public class Note
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public User User { get; set; }
    }
}
