using System;
using System.Threading.Tasks;
using AutoMapper;
using MessengerAPI.Constants;
using MessengerAPI.Data;
using MessengerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.User)]
    public class MessagesController : ControllerBase
    {
        private readonly MessagesManager _messagesManager;

        public MessagesController(Model context, IMapper mapper)
        {
            _messagesManager = new MessagesManager(context, mapper);
        }

        /// <summary>
        /// Get messages related to chat.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetChatMessages/{id}")]
        public async Task<IActionResult> GetChatMessages(Guid chatId)
        {
            var messages = await _messagesManager.GetMessagesByChatId(chatId);

            return Ok(messages);
        }
    }
}