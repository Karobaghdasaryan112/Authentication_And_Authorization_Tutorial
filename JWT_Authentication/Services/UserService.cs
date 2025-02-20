using JWT_Authentication.Entities;
using JWT_Authentication.Interfaces.Repositories;
using JWT_Authentication.Interfaces.Services;
using JWT_Authentication.Interfaces.Utils;
using JWT_Authentication.ViewModels;

namespace JWT_Authentication.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository { get; set; }
        private IJwtService _jwtService { get; set; }
        public UserService(
            IUserRepository userRepository,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<bool> UserRegistration(User user)
        {
            var ExistingUser = await _userRepository.GetUserByUserName(user.UserName);

            if (ExistingUser != null)
            {
                await _userRepository.CreateEntity(user);
                return true;
            }

            return false;
        }

        public async Task<string> UserLogin(UserLoginViewModel user)
        {
            var ExistingUser = await _userRepository.GetUserByUserName(user.UserName);

            if (ExistingUser != null)
            {
                if (PasswordHasher.VerifyPassword(user.Password,ExistingUser.PasswordHash))
                {
                    return await _jwtService.GenerateToken(ExistingUser);
                }
            }
            return null;
        }
    }
}
