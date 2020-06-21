using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.Chats;
using MessengerAPI.Data.DataTransferObjects.Messages;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Services.HelperClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Services
{
    public class MessagesManager
    {
        private readonly Model _db;
        private readonly IMapper _mapper;
        private readonly LogsManager _logsManager;
        private readonly ChatsManager _chatsManager;

        public MessagesManager(Model context, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager, 
            IOptions<ApplicationSettings> appSettings)
        {
            _db = context;
            _mapper = mapper;
            _logsManager = new LogsManager(context);
            _chatsManager = new ChatsManager(context, mapper, userManager, appSettings);
        }


        /// <summary>
        /// Get messages by chat id.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<ICollection<Message>> GetMessagesByChatId(Guid chatId)
        {
            return await _db.Messages
                .Where(x => x.Chat.ChatId == chatId)
                .ToListAsync();
        }


        /// <summary>
        /// Find message by message id.
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task<Message> FindMessage(Guid messageId)
        {
            var message = await _db.Messages.FindAsync(messageId);

            if (message == null) return null;

            return message;
        }

        /// <summary>
        /// Process message
        /// to existing chat.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        public async Task<MessageResponse> ProcessChatMessage(PostMessageToChatDto messageData)
        {
            var message = _mapper.Map<Message>(messageData);

            var messageSaved = await SaveMessage(message);

            if (!messageSaved) return MessageResponse.Unsuccessful("Error in saving message.");

            return MessageResponse.Successfull(message);
        }

        /// <summary>
        /// Process new message.
        /// 1. Create new chat with message.
        /// 2. Send message.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        public async Task<MessageResponse> ProcessNewMessage(PostNewMessageDto messageData)
        {                
            var message = _mapper.Map<Message>(messageData);

            var messageSaved = await SaveMessage(message);

            if (!messageSaved) return MessageResponse.Unsuccessful("Error in saving message.");

            var chatResponse = await _chatsManager.ProcessNewChat(messageData, message);

            if(!chatResponse.Success) return MessageResponse.Unsuccessful("Error creating chat.");

            return MessageResponse.Successfull(message);
        }

        /// <summary>
        /// Saves message to database.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>
        /// True if message successfully saved.
        /// </returns>
        private async Task<bool> SaveMessage(Message message)
        {
            _db.Messages.Add(message);

            try
            {
                var result = await _db.SaveChangesAsync();

                if (result == 1) return true;
            }
            catch (Exception ex)
            {
                _logsManager.SaveLog(message, ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Arhives message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>
        /// True if message is successfully arhived.
        /// </returns>
        public async Task<bool> ArchiveMessage(Message message)
        {
            message.Archived = true;

            try
            {
                var result = await _db.SaveChangesAsync();

                if (result == 1) return true;
            }
            catch (Exception ex)
            {

                _logsManager.SaveLog(message, ex.Message);
            }

            return false;
        }

    }
}
