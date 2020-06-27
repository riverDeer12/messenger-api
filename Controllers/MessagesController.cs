using System;
using System.Threading.Tasks;
using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.Messages;
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
    public class MessagesController : ControllerBase
    {
        private readonly MessagesManager _messagesManager;
        private readonly IHubContext<MessageHub> _hub;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(IOptions<ApplicationSettings> appSettings,
            Model context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IHubContext<MessageHub> hub
            )
        {
            _messagesManager = new MessagesManager(context, mapper, userManager, appSettings);
            _userManager = userManager;
            _hub = hub;
        }

        [HttpGet]
        [Route("GetUserMessages")]
        public async Task<IActionResult> GetUserMessages()
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId)) return BadRequest();

            var response = await _messagesManager.GetUserMessages(userId);

            return Ok(response.Messages);
        }

        /// <summary>
        /// Get messages by chat id.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns>
        /// Status with list of messages.
        /// </returns>
        [HttpGet]
        [Route("GetChatMessages/{id}")]
        public async Task<IActionResult> GetChatMessages(Guid chatId)
        {
            var response = await _messagesManager.GetChatMessages(chatId);

            return Ok(response.Messages);
        }

        /// <summary>
        /// Process new message 
        /// to existing chat.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendChatMessage")]
        public async Task<IActionResult> SendChatMessage(PostMessageToChatDto messageData)
        {
            var response = await _messagesManager.ProcessChatMessage(messageData);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            await _hub.Clients.All.SendAsync("sendmessage", response.Message);

            return Ok();
        }

        /// <summary>
        /// Process new message
        /// to new chat.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendNewChatMessage")]
        public async Task<IActionResult> SendNewChatMessage(PostNewMessageDto messageData)
        {
            var chatOwnerId = User.FindFirst("UserId")?.Value;

            var chatOwner = await _userManager.FindByIdAsync(chatOwnerId);

            var response = await _messagesManager.ProcessNewMessage(messageData, chatOwner);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            await _hub.Clients.All.SendAsync("receivemessages", response.Message);

            return Ok();
        }


    }
}