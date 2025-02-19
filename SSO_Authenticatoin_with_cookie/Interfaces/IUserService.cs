using SSO_Authenticatoin_with_cookie.Entity;

namespace SSO_Authenticatoin_with_cookie.Interfaces
{
    public interface IUserService
    {
        Task UserRegistration(User user);
    }
}
