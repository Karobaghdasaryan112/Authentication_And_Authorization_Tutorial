using JWT_Authentication.Data;
using JWT_Authentication.Entities;
using JWT_Authentication.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JWT_Authentication.Repositories
{
    public class UserRepository : IUserRepository
    {
        private JwtDbContext _context;
        public UserRepository(JwtDbContext context)
        {
            _context = context;
        }
        public Task<bool> CreateEntity(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEntityById(int EntityId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetEntityById(int entityId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByUserName(string UserName)
        {
            return await _context.Users.Where(u => u.UserName == UserName).Include(U => U.Roles).FirstOrDefaultAsync();
        }

        public Task UpdateEntity(User entity, int Id)
        {
            throw new NotImplementedException();
        }
    }
}
