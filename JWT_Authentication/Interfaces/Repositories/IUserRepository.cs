using JWT_Authentication.Entities;

namespace JWT_Authentication.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByUserName(string UserName);
    }
}
