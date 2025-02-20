using JWT_Authentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWT_Authentication.Data
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                    Id = 1,
                    RoleName = "Admin"
                },
                new Role
                {
                    Id = 2,
                    RoleName = "User"
                },
                new Role
                {
                    Id = 3,
                    RoleName = "Meneger"
                }
            );
        }
    }
}
