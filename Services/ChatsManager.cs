using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Services
{
    public class ChatsManager
    {
        private readonly Model _db;
        private readonly IMapper _mapper;

        public ChatsManager(Model context, IMapper mapper)
        {
            _db = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all active chats
        /// for logged user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<ICollection<Chat>> GetChatsForUser(string userId)
        {
            var chats = await GetActiveChats(userId);

            return chats;
        }

        /// <summary>
        /// Get all active chats for user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<ICollection<Chat>> GetActiveChats(string userId)
        {
            var chats = new List<Chat>();

            var userChats = await GetAllChatsForUser(userId);

            foreach (var userChat in userChats)
            {
                var chat = await FindChatById(userChat.ChatId);

                if (chat == null) continue;

                chats.Add(chat);
            }

            return chats;
        }

        /// <summary>
        /// Find chat by chat id 
        /// parameter.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task<Chat> FindChatById(Guid chatId)
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
        private async Task<List<ApplicationUserChat>> GetAllChatsForUser(string userId)
        {
            return await _db.UserChats
                .Where(x => x.UserId == userId && x.Archived == false)
                .ToListAsync();
        }
    }
}
