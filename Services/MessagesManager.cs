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
    public class MessagesManager
    {
        private readonly Model _db;
        private readonly IMapper _mapper;

        public MessagesManager(Model context, IMapper mapper)
        {
            _db = context;
            _mapper = mapper;
        }


        /// <summary>
        /// Get message by chat id.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<ICollection<Message>> GetMessagesByChatId(Guid chatId)
        {
            return await _db.Messages
                .Where(x => x.Chat.ChatId == chatId)
                .ToListAsync();
        }
    }
}
