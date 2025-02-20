using JWT_Authentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JWT_Authentication.Data
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasData(
                new UserRole
                {
                    UserId = 1,
                    RoleId = 1,

                },
                new UserRole
                {
                    UserId = 2,
                    RoleId = 2
                }
                );


        }
    }
}
