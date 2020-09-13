using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessengerAPI.Constants;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Services;
using MessengerAPI.Services.ResponseClasses;
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
        [AllowAnonymous]
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
        [Authorize]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirst("UserId")?.Value;

            var userDetails = await _usersManager.GetUserDetails(userId);

            if (userDetails == null) return BadRequest();

            return Ok(userDetails);
        }

        /// <summary>
        /// Update users data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateUserDetails")]
        [Authorize]
        public async Task<IActionResult> UpdateUserDetails(UserDetailsDto data)
        {
            var userId = User.FindFirst("UserId")?.Value;

            var response = await _usersManager.UpdateUserDetails(data, userId);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            return Ok();
        }

        /// <summary>
        /// Get all active 
        /// users for search.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSearchUsers")]
        public async Task<IActionResult> GetSearchUsers()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest("Logged User Not Found.");

            var response = await _usersManager.GetSearchUsers(userId);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            return Ok(response.Users);
        }

    }
}