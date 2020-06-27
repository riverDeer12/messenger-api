﻿using MessengerAPI.Data;
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
    public class ChatsRepository
    {
        private readonly Model _db;
        private readonly LogsManager _logsManager;

        public ChatsRepository(Model context)
        {
            _db = context;
            _logsManager = new LogsManager(context);
        }

        /// <summary>
        /// Save new chat to database.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>
        /// True if successfully saved.
        /// </returns>
        public async Task<bool> SaveChat(Chat chat)
        {
            _db.Chats.Add(chat);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logsManager.SaveLog(chat, ex.Message);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Get archived chats
        /// for logged user by
        /// userid.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ICollection<ApplicationUserChat>> GetArchivedChatsByUserId(string userId)
        {
            return await _db.UserChats
                .Where(x => x.UserId == userId && x.Archived == false)
                .ToListAsync();
        }

        /// <summary>
        /// Find chat by chat id 
        /// parameter.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<Chat> FindChatById(Guid chatId)
        {
            return await _db.Chats
                 .FirstOrDefaultAsync(x => x.ChatId == chatId);
        }

        /// <summary>
        /// Get all chats that
        /// user was or is in.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<ApplicationUserChat>> GetActiveChatsByUserId(string userId)
        {
            return await _db.UserChats
                .Where(x => x.UserId == userId && x.Archived == false)
                .ToListAsync();
        }

        /// <summary>
        /// Find chat by chatId.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<ChatResponse> FindChatByChatId(Guid chatId)
        {
            var chat = await _db.Chats
                .Include("Messages")
                .FirstOrDefaultAsync(x => x.ChatId == chatId);

            if (chatId == null) return ChatResponse.Unsuccessful("Chat not found.");

            return ChatResponse.Successfull(chat);
        }
    }
}
