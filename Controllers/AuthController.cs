using System.Threading.Tasks;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthManager _authManager;

        public AuthController(IOptions<ApplicationSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _authManager = new AuthManager(appSettings, userManager);
        }

        /// <summary>
        /// Login user into application
        /// based on login data.
        /// </summary>
        /// <param name="loginData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDto loginData)
        {
            var user = await _authManager.FindByUserName(loginData.UserName);

            if (!await _authManager.AuthorizeUser(user, loginData.Password)) return Unauthorized();

            var token = await _authManager.GenerateJwtToken(user);

            return Ok(new { token });
        }
    }
}