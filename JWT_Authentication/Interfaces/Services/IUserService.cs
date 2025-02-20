
using JWT_Authentication.Entities;
using JWT_Authentication.ViewModels;

namespace JWT_Authentication.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> UserRegistration(User user);

        Task<string> UserLogin(UserLoginViewModel user);  
    }
}
