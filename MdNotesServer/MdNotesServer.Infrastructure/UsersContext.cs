using MdNotesServer.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MdNotesServer.Infrastructure
{
    public class UsersContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<Note> Notes {get; set;}

        public UsersContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}
