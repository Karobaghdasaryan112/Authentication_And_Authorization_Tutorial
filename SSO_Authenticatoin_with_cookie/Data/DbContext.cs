using Microsoft.EntityFrameworkCore;
using SSO_Authenticatoin_with_cookie.Entity;

namespace SSO_Authenticatoin_with_cookie.Data
{
    public class DataProtectionDbContext : DbContext
    {
        public DataProtectionDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<DataProtectionKeys> DataProtectionKeys { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
