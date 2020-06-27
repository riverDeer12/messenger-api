using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Hubs;
using MessengerAPI.Services;
using MessengerAPI.Services.HelperClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly ChatsManager _chatsManager;

        public ChatsController(Model context, 
            IMapper mapper, 
            IHubContext<ChatHub> hub,
            UserManager<ApplicationUser> userManager,
            IOptions<ApplicationSettings> appSettings)
        {
            _hub = hub;
            _chatsManager = new ChatsManager(context, mapper, userManager, appSettings);
        }

        /// <summary>
        /// Get chat with all
        /// associated messages.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetChat")]
        public async Task<IActionResult> GetChat(Guid chatId)
        {
            var response = await _chatsManager.GetChat(chatId);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            return Ok(response.Chat);
        }

        /// <summary>
        /// Get active chats
        /// for logged user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetActiveChats")]
        public async Task<IActionResult> GetActiveChats()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest("Logged User Not Found.");

            var response = await _chatsManager.GetActiveChats(userId);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            return Ok(response.Chats);
        }

        /// <summary>
        /// Get active chats
        /// for logged user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetArchivedChats")]
        public async Task<IActionResult> GetArchivedChats()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest("Logged User Not Found.");

            var response = await _chatsManager.GetArchivedChats(userId);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            return Ok(response.Chats);
        }
    }
}
