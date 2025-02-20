using JWT_Authentication.Entities;
using JWT_Authentication.Interfaces.Services;
using JWT_Authentication.Interfaces.Utils;
using JWT_Authentication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IHttpContextAccessor _httpContextAccessor;
        public UsersController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationViewModel viewModel)
        {
            var newUser = new User
            {
                Email = viewModel.Email,
                UserName = viewModel.UserName,
                PasswordHash = PasswordHasher.HashPassword(viewModel.Password),
                Roles = new List<Role>
                {
                    new Role
                    {
                        RoleName = "User"
                    }
                }
            };

            return (await _userService.UserRegistration(newUser))
                 ? Ok("User registered successfully")
                 : BadRequest("User already exists");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginViewModel viewModel)
        {

            var Token = await _userService.UserLogin(viewModel);

            _httpContextAccessor.HttpContext.Response.Cookies.Append("access_token", Token, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                
            });

            return Token != null
                ? Ok(new { Token })
                : BadRequest("Invalid username or password");
        }
        [HttpPost("UserName")]
        [Authorize]
        public IActionResult GetUserName()
        {
            return Ok(User.Identity.Name);
        }
    }
}
