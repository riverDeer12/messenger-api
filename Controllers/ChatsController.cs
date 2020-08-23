using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.Chats;
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
        private readonly IHubContext<MessageHub> _hub;
        private readonly ChatsManager _chatsManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatsController(Model context, 
            IMapper mapper, 
            IHubContext<MessageHub> hub,
            UserManager<ApplicationUser> userManager,
            IOptions<ApplicationSettings> appSettings)
        {
            _hub = hub;
            _chatsManager = new ChatsManager(context, mapper, userManager, appSettings);
            _userManager = userManager;
        }

        /// <summary>
        /// Get chat with all
        /// associated messages.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetChat/{chatId}")]
        public async Task<IActionResult> GetChat([FromRoute] string chatId)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest("Logged User Not Found.");

            var user = await _userManager.FindByIdAsync(userId);

            var realChatId = Guid.Parse(chatId);

            var response = await _chatsManager.GetChat(realChatId, user);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            return Ok(response.ChatDetails);
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

            var user = await _userManager.FindByIdAsync(userId);

            var response = await _chatsManager.GetActiveChats(user);

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

        /// <summary>
        /// Join user to chat.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("JoinChat/{connectionId}/{chatId}")]
        public async Task<IActionResult> JoinChat([FromRoute] string connectionId, [FromRoute] string chatId)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest("Logged User Not Found.");

            var response = await _chatsManager.JoinUserToChat(chatId, userId);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            await _hub.Groups.AddToGroupAsync(connectionId, chatId);

            return Ok();
        }

        /// <summary>
        /// Remove user from chat.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LeaveChat/{connectionId}/{chatId}")]
        public async Task<IActionResult> LeaveChat([FromRoute] string connectionId, [FromRoute] string chatId)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest("Logged User Not Found.");

            var response = await _chatsManager.RemoveUserFromChat(chatId, userId);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            await _hub.Groups.RemoveFromGroupAsync(connectionId, chatId);

            return Ok();
        }

        /// <summary>
        /// Create new chat.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostNewChat")]
        public async Task<IActionResult> PostNewChat(NewChatDto data)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest("Logged User Not Found.");

            data.AdminId = userId;

            var response = await _chatsManager.ProcessNewChat(data);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            var chatId = response.Chat.ChatId.ToString();

            return Ok(new { chatId });
        }
    }
}
