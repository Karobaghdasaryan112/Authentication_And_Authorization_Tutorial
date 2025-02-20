using JWT_Authentication.Entities;
using JWT_Authentication.Interfaces.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWT_Authentication.Data
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            var FirstUserPassword = PasswordHasher.HashPassword("karokar3");
            var SecondUserPassword = PasswordHasher.HashPassword("adminadmin");

            builder.HasData(
                new User
                {
                    Id = 1,
                    UserName = "karo",
                    PasswordHash = FirstUserPassword,
                    Email = "karobaghdasaryan7@gmail.com"
                },
                new User
                {
                    Id = 2,
                    UserName = "admin",
                    PasswordHash = SecondUserPassword,
                    Email = "admin@gmail.com"
                });

            builder
             .HasMany(u => u.Roles)
             .WithMany(r => r.Users)
             .UsingEntity<UserRole>(userRole =>
             userRole.HasOne(ur => ur.Role)
                 .WithMany()
                 .HasForeignKey(ur => ur.RoleId),
             userRole =>
             userRole.HasOne(ur => ur.User)
                 .WithMany()
                 .HasForeignKey(ur => ur.UserId),
             userRole =>
             {
                 userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
             });
        }
    }
}
