﻿using AutoMapper;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesManager(Model context, 
            IMapper mapper,
            UserManager<ApplicationUser> userManager, 
            IOptions<ApplicationSettings> appSettings)
        {
            _mapper = mapper;
            _userManager = userManager;
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
        /// 
        /// </summary>
        /// <param name="messageData"></param>
        /// <returns></returns>
        public async Task<MessageResponse> ProcessChatMessage(PostMessageToChatDto messageData)
        {
            var messageOwner = await _userManager.FindByIdAsync(messageData.UserId);

            if(messageOwner == null) return MessageResponse.Unsuccessful("Error finding message owner.");

            messageData.ApplicationUser = messageOwner;

            var message = _mapper.Map<Message>(messageData);

            var messageSaved = await _repository.SaveMessage(message);

            if (!messageSaved) return MessageResponse.Unsuccessful("Error in saving message.");

            var chatResponse = await _chatsManager.RefreshChatActivity(messageData.ChatId);

            if (!chatResponse.Success) return MessageResponse.Unsuccessful(chatResponse.ErrorMessage);

            var messageDetailsDto = _mapper.Map<MessageDetailsDto>(message);

            messageDetailsDto.ApplicationUser = messageOwner.UserName;

            return MessageResponse.Successfull(messageDetailsDto);
        }

        public async Task<MessageResponse> GetChatMessages(Guid chatId)
        {
            var messages = await _repository.GetMessagesByChatId(chatId);

            return MessageResponse.Successfull(messages);
        }
    }
}
