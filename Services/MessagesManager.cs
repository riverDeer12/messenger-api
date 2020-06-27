using AutoMapper;
using MessengerAPI.Data;
using MessengerAPI.Data.DataTransferObjects.Messages;
using MessengerAPI.Data.Models;
using MessengerAPI.Helpers;
using MessengerAPI.Repositories;
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
        private readonly IMapper _mapper;
        private readonly ChatsManager _chatsManager;
        private readonly MessagesRepository _repository;

        public MessagesManager(Model context, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager, 
            IOptions<ApplicationSettings> appSettings)
        {
            _mapper = mapper;
            _repository = new MessagesRepository(context);
            _chatsManager = new ChatsManager(context, mapper, userManager, appSettings);
        }

        /// <summary>
        /// Get all messages
        /// for logged user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<MessageResponse> GetUserMessages(string userId)
        {
            var userMessages = await _repository.GetMessagesByUserId(userId);

            return MessageResponse.Successfull(userMessages);
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

            var messageSaved = await _repository.SaveMessage(message);

            if (!messageSaved) return MessageResponse.Unsuccessful("Error in saving message.");

            var messageDetailsDto = _mapper.Map<MessageDetailsDto>(message);

            return MessageResponse.Successfull(messageDetailsDto);
        }

        public async Task<MessageResponse> GetChatMessages(Guid chatId)
        {
            var messages = await _repository.GetMessagesByChatId(chatId);

            return MessageResponse.Successfull(messages);
        }

        /// <summary>
        /// Process new message.
        /// 1. Create new chat with message.
        /// 2. Send message.
        /// </summary>
        /// <param name="messageData"></param>
        /// <param name="chatOwner"></param>
        /// <returns></returns>
        public async Task<MessageResponse> ProcessNewMessage(PostNewMessageDto messageData, ApplicationUser chatOwner)
        {
            var chatResponse = await _chatsManager.ProcessNewChat(messageData);

            if (!chatResponse.Success) return MessageResponse.Unsuccessful("Error creating chat.");

            var message = _mapper.Map<Message>(messageData);

            message.Chat = chatResponse.Chat;

            message.ApplicationUser = chatOwner;

            var messageSaved = await _repository.SaveMessage(message);

            if (!messageSaved) return MessageResponse.Unsuccessful("Error in saving message.");

            var messageDetailsDto = _mapper.Map<MessageDetailsDto>(message);

            return MessageResponse.Successfull(messageDetailsDto);
        }
    }
}
