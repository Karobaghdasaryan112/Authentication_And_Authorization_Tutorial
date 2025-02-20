using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SSO_Authenticatoin_with_cookie.Entity;

namespace SSO_Authenticatoin_with_cookie.Data
{
    public class DataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        public DataProtectionDbContext(DbContextOptions options) : base(options)
        {

        }
         public DbSet<DataProtectionKey> ProtectionKeys { get; set; }
         public DbSet<User> Users { get; set; }

         public DbSet<DataProtectionKey> DataProtectionKeys => ProtectionKeys;

            
    }
}
