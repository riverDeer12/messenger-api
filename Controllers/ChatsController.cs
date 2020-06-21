using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Hubs;
using MessengerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private IHubContext<ChatHub> _hub;
        private ChatsManager _chatsManager;

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
        /// Get chats for 
        /// logged user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetChatMessages()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest();

            _hub.Clients.All.SendAsync("chats", _chatsManager.GetUserChatsById(userId));

            return Ok(new { Message = "Messages delivered." });
        }
    }
}
