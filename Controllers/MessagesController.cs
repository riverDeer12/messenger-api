using System;
using System.Threading.Tasks;
using AutoMapper;
using MessengerAPI.Constants;
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
        private IHubContext<ChatHub> _hub;

        public MessagesController(IOptions<ApplicationSettings> appSettings,
            Model context,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IHubContext<ChatHub> hub
            )
        {
            _messagesManager = new MessagesManager(context, mapper, userManager, appSettings);
            _hub = hub;
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
            var messages = await _messagesManager.GetMessagesByChatId(chatId);

            return Ok(messages);
        }

        /// <summary>
        /// Process message to
        /// existing chat.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ProcessChatMessage")]
        public async Task<IActionResult> ProcessChatMessage(PostMessageToChatDto messageData)
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
        [Route("ProcessNewMessage")]
        public async Task<IActionResult> ProcessNewMessage(PostNewMessageDto messageData)
        {
            var response = await _messagesManager.ProcessNewMessage(messageData);

            if (!response.Success) return BadRequest(response.ErrorMessage);

            await _hub.Clients.All.SendAsync("sendmessage", response.Message);

            return Ok();
        }


    }
}