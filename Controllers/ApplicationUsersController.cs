using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessengerAPI.Constants;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationUsersController : ControllerBase
    {
        private readonly ApplicationUsersManager _usersManager;

        public ApplicationUsersController(
            IOptions<ApplicationSettings> appSettings,
            Model context,
            UserManager<ApplicationUser> userManager, 
            IMapper mapper)
        {
            _usersManager = new ApplicationUsersManager(context, mapper, userManager, appSettings);
        }

        /// <summary>
        /// Register new user to application.
        /// </summary>
        /// <param name="registrationData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegistrationDto registrationData)
        {
            var response = await _usersManager.RegisterUser(registrationData);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            return Ok(new { response.Token });
        }

        /// <summary>
        /// Register new application admin.
        /// Will be deleted before production.
        /// </summary>
        /// <param name="registrationData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin(RegistrationDto registrationData)
        {
            var success = await _usersManager.RegisterAdmin(registrationData);

            if (!success) return BadRequest(registrationData);

            return Ok();
        }

        /// <summary>
        /// Get user's details
        /// for profile by token.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserDetails")]
        [Authorize(Roles = Roles.User)]
        public string GetUserDetails()
        {
            return "something";
        }

    }
}