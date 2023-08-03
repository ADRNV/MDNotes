using MdNotesServer.Infrastructure.Entities;
using MdNotesServer.Infrastructure.EntityesConfiguration;
using Microsoft.EntityFrameworkCore;

namespace MdNotesServer.Infrastructure
{
    public class NotesContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Note> Notes { get; set; }

        public NotesContext(DbContextOptions<NotesContext> options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.ApplyConfiguration(new NoteConfiguration());
        }
    }
}
