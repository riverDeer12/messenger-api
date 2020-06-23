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
    public class ChatsManager
    {
        private readonly Model _db;
        private readonly IMapper _mapper;
        private readonly ApplicationUsersManager _usersManager;
        private readonly LogsManager _logsManager;

        public ChatsManager(Model context, IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            IOptions<ApplicationSettings> appSettings)
        {
            _db = context;
            _mapper = mapper;
            _usersManager = new ApplicationUsersManager(context, mapper, userManager, appSettings);
            _logsManager = new LogsManager(context);
        }

        /// <summary>
        /// Get all active chats
        /// for logged user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ICollection<Chat>> GetUserChatsById(string userId)
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

        /// <summary>
        /// Process new chat.
        /// </summary>
        /// <param name="newMessageData"></param>
        /// <returns></returns>
        public async Task<ChatResponse> ProcessNewChat(PostNewMessageDto newMessageData)
        {
            var users = await GetChatUsers(newMessageData);

            var newChatData = new NewChatDto(users);

            var chat = _mapper.Map<Chat>(newChatData);

            chat.ApplicationUserChats = SetChatUsers(chat, users);

            var success = await SaveChat(chat);

            if (!success) return ChatResponse.Unsuccessful("Error saving chat.");

            return ChatResponse.Successfull(chat);
        }

        /// <summary>
        /// Helper method for
        /// assigning all users to
        /// newly created chat.
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        private ICollection<ApplicationUserChat> SetChatUsers(Chat chat, List<ApplicationUser> users)
        {
            var chatUsers = new List<ApplicationUserChat>();

            foreach(var user in users)
            {
                var newChatUser = new ApplicationUserChat
                {
                    ApplicationUser = user,
                    Chat = chat
                };

                chatUsers.Add(newChatUser);
            }

            return chatUsers;
        }

        /// <summary>
        /// Make list of chat users
        /// for new chat. Contains
        /// all receivers and chat owner.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        public async Task<List<ApplicationUser>> GetChatUsers(PostNewMessageDto messageData)
        {
            var chatUsers = new List<ApplicationUser>();

            foreach(var receiverId in messageData.ReceiverIds)
            {
                var receiver = await _usersManager.FindUserById(receiverId);

                if (receiver == null) continue;

                chatUsers.Add(receiver);
            }

            var chatOwner = await _usersManager.FindUserById(messageData.UserId);

            chatUsers.Add(chatOwner);

            return chatUsers;
        }

        /// <summary>
        /// Save new chat to database.
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns>
        /// True if successfully saved.
        /// </returns>
        private async Task<bool> SaveChat(Chat chat)
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
    }
}
