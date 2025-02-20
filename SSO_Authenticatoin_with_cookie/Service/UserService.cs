using Microsoft.EntityFrameworkCore;
using SSO_Authenticatoin_with_cookie.Data;
using SSO_Authenticatoin_with_cookie.Entity;
using SSO_Authenticatoin_with_cookie.Helpers;
using SSO_Authenticatoin_with_cookie.Interfaces;

namespace SSO_Authenticatoin_with_cookie.Service
{

    public class UserService : IUserService
    {
        private DataProtectionDbContext _dataProtectionDbContext;
        public UserService(DataProtectionDbContext dataProtectionDbContext)
        {
            _dataProtectionDbContext = dataProtectionDbContext;
        }

        public async Task UserRegistration(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var PassowrdHash = PasswordHasher.HashPassword(user.Password);

            user.Password = PassowrdHash;

            var ExistingUser = await _dataProtectionDbContext.Users.Where(U => U.UserName == user.UserName).FirstOrDefaultAsync();

            if (ExistingUser == null)
            {
                await _dataProtectionDbContext.AddAsync(user);
                await _dataProtectionDbContext.SaveChangesAsync();
            }
        }

    }
}
