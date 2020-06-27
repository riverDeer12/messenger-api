using MessengerAPI.Data;
using MessengerAPI.Data.Models;
using MessengerAPI.Services;
using MessengerAPI.Services.HelperClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Repositories
{
    public class MessagesRepository
    {
        private readonly Model _db;
        private readonly LogsManager _logsManager;

        public MessagesRepository(Model context)
        {
            _db = context;
            _logsManager = new LogsManager(context);
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
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logsManager.SaveLog(message, ex.Message);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves message to database.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>
        /// True if message successfully saved.
        /// </returns>
        public async Task<bool> SaveMessage(Message message)
        {
            _db.Messages.Add(message);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logsManager.SaveLog(message, ex.Message);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Get all messages for user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<Message>> GetMessagesByUserId(string userId)
        {
            return await _db.Messages
                .Where(x => x.ApplicationUser.Id == userId)
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
        /// Get messages by chat id.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<List<Message>> GetMessagesByChatId(Guid chatId)
        {
            return await _db.Messages
                .Where(x => x.Chat.ChatId == chatId)
                .ToListAsync();
        }

    }
}
