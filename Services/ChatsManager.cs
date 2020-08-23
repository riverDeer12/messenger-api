using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.DataTransferObjects.Chats;
using MessengerAPI.Data.DataTransferObjects.Messages;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Repositories;
using MessengerAPI.Services.HelperClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Services
{
    public class ChatsManager
    {
        private readonly IMapper _mapper;
        private readonly ApplicationUsersManager _appUsersManager;
        private readonly ChatsRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChatsManager(Model context, IMapper mapper, 
            UserManager<ApplicationUser> userManager, 
            IOptions<ApplicationSettings> appSettings)
        {
            _mapper = mapper;
            _repository = new ChatsRepository(context);
            _userManager = userManager;
            _appUsersManager = new ApplicationUsersManager(context, mapper, userManager, appSettings);
        }

        /// <summary>
        /// Get all active chats for user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ChatResponse> GetActiveChats(ApplicationUser user)
        {
            var chats = new List<Chat>();

            var userChats = await _repository.GetActiveChatsByUserId(user.Id);

            foreach (var userChat in userChats)
            {
                var chat = await _repository.FindChatById(userChat.ChatId);

                if (chat == null) 
                    return ChatResponse.Unsuccessful("Error finding chat.");

                chat.Name = await SetChatName(chat, user);

                chats.Add(chat);
            }

            chats = chats
                .OrderByDescending(x => x.LastActivityAt)
                .ToList();

            return ChatResponse.Successfull(chats);
        }

        /// <summary>
        /// Handle situation when chat
        /// name is not specified. Then
        /// go through users (except request user)
        /// and get their usernames 
        /// separated by comma.
        /// </summary>
        /// <param name="chat"></param>
        /// <param name="userToRemove"></param>
        /// <returns></returns>
        private async Task<string> SetChatName(Chat chat, ApplicationUser userToRemove)
        {
            var chatName = "";

            if (!string.IsNullOrEmpty(chat.Name)) return chat.Name;

            var chatUsers = await _repository.GetActiveUsersByChatId(chat.ChatId);

            var users = _repository.GetChatUsers(chatUsers);

            var index = users.IndexOf(userToRemove);

            users.RemoveAt(index);

            foreach(var user in users)
            {  
                chatName += user.UserName + ", ";
            }

            return chatName.Substring(0, chatName.Length - 2);
        }

        /// <summary>
        /// Get chat by
        /// sent chatId.
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="loggedUser"></param>
        /// <returns></returns>
        public async Task<ChatResponse> GetChat(Guid chatId, ApplicationUser loggedUser)
        {
            var response = await _repository.FindChatByChatId(chatId);

            if (!response.Success) return ChatResponse.Unsuccessful(response.ErrorMessage);

            response.Chat.Name = await SetChatName(response.Chat, loggedUser);

            var chatDetails = _mapper.Map<ChatDetailsDto>(response.Chat);

            var chatUsers = _repository.GetChatUsers(response.Chat.ApplicationUserChats);

            chatUsers.Remove(loggedUser);

            var users = _mapper.Map<List<UserDetailsDto>>(chatUsers);

            chatDetails.Users = users;

            return ChatResponse.Successfull(chatDetails);
        }

        /// <summary>
        /// Update last activity
        /// for chat after new 
        /// received message.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public async Task<ChatResponse> RefreshChatActivity(Guid chatId)
        {
            var chat = await _repository.FindChatById(chatId);

            if (chat == null) return ChatResponse.Unsuccessful("Chat not found.");

            chat.LastActivityAt = DateTime.Now;

            var success = await _repository.UpdateChat(chat);

            if(!success) return ChatResponse.Unsuccessful("Error updating chat.");

            return ChatResponse.Successfull();
        }


        /// <summary>
        /// Join user to chat that 
        /// already exists.
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ChatResponse> JoinUserToChat(string chatId, string userId)
        {
            var realChatId = Guid.Parse(chatId);

            var userChat = new ApplicationUserChat
            {
                ChatId = realChatId,
                UserId = userId
            };

            var alreadyJoined = _repository.CheckUserChatExistance(userChat);

            if(alreadyJoined) return ChatResponse.Successfull();

            var success = await _repository.SaveUserChat(userChat);

            if (!success) return ChatResponse.Unsuccessful("Error saving userchat relation.");

            return ChatResponse.Successfull();
        }

        /// <summary>
        /// Get archived chats
        /// for logged user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ChatResponse> GetArchivedChats(string userId)
        {
            var chats = new List<Chat>();

            var userChats = await _repository.GetArchivedChatsByUserId(userId);

            foreach (var userChat in userChats)
            {
                var chat = await _repository.FindChatById(userChat.ChatId);

                if (chat == null)
                    return ChatResponse.Unsuccessful("Error finding chat.");

                chats.Add(chat);
            }

            return ChatResponse.Successfull(chats);
        }

        /// <summary>
        /// Remove user from 
        /// related chat.
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ChatResponse> RemoveUserFromChat(string chatId, string userId)
        {
            var realChatId = Guid.Parse(chatId);

            var userChat = new ApplicationUserChat
            {
                ChatId = realChatId,
                UserId = userId
            };

            var success = await _repository.DeleteUserChat(userChat);

            if (!success) return ChatResponse.Unsuccessful("Error removing userchat relation.");

            return ChatResponse.Successfull();
        }

        /// <summary>
        /// Process new chat.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<ChatResponse> ProcessNewChat(NewChatDto data)
        {
            var users = await GetChatUsers(data.Users, data.AdminId);

            data.ApplicationUsers = users;

            var chat = _mapper.Map<Chat>(data);

            chat.ApplicationUserChats = SetChatUsers(chat, users);

            var success = await _repository.SaveChat(chat);

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
        /// Chat owner is always set as
        /// first element in list.
        /// </summary>
        /// <param name="users"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public async Task<List<ApplicationUser>> GetChatUsers(List<UserDetailsDto> users, string adminId)
        {
            var finalChatUsers = new List<ApplicationUser>();

            var admin = await _appUsersManager.FindUserById(adminId);

            finalChatUsers.Add(admin);

            foreach(var user in users)
            {
                var chatUser = await _appUsersManager.FindUserById(user.Id);
                finalChatUsers.Add(chatUser);
            }

            return finalChatUsers;
        }
    }
}
