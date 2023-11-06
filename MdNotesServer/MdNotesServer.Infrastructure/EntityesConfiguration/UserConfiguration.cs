using MdNotesServer.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MdNotesServer.Infrastructure.EntityesConfiguration
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(u => u.Notes)
                .WithOne(n => n.User);

            builder.HasData(new User
            {
                Id = Guid.NewGuid(),
                UserName = "User",
                NormalizedUserName = "USER",
                Email = "userexample@gmail.com",
                NormalizedEmail = "userexample@gmail.com".ToUpper(),
                PasswordHash = "AQAAAAIAAYagAAAAEJnbSpz4UNKDnq+KMzOiivLwPovhKV3SwHz8w95dQwvQWKbmo5yZa4NKTSpf8U6Muw==",
                SecurityStamp = Guid.NewGuid().ToString(),
            });
        }
    }
}
