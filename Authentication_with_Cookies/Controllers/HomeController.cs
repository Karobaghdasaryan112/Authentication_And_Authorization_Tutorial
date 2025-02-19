using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authentication_with_Cookies.Controllers
{   
    public class HomeController : Controller
    {

        [HttpPost("/mvc/login")]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignInAsync("default", new ClaimsPrincipal(
                
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString())
                    }, "custom_auth_type")
                )
            );
            return Ok();
        }

        [HttpPost("get/User")]
        [Authorize]
        public async Task<IActionResult> UserName()
        {
            return Ok();
        }
    }
}
