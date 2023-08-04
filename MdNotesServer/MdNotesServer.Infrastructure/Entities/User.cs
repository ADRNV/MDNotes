using Microsoft.AspNetCore.Identity;

namespace MdNotesServer.Infrastructure.Entities
{
    public class User : IdentityUser<Guid>
    {
        public List<Note>? Notes { get; set; }
    }
}
