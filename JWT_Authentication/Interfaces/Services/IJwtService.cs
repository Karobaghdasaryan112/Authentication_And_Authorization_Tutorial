using JWT_Authentication.Entities;

namespace JWT_Authentication.Interfaces.Services
{
    public interface IJwtService
    {
        Task<string> GenerateToken(User user);

        Task<bool> VerifyToken(string token);
    }
}
