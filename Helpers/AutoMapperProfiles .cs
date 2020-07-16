using AutoMapper;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.DataTransferObjects.Chats;
using MessengerAPI.Data.DataTransferObjects.Messages;
using MessengerAPI.Data.Models;
using System.Collections.Generic;

namespace MessengerAPI.Helpers
{
    public class AutoMapperProfiles : Profile 
    {
        public AutoMapperProfiles()
        {
            //-- application user profiles --//
            CreateMap<RegistrationDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDetailsDto>();
            CreateMap<UserDetailsDto, ApplicationUser>();

            //-- chat profiles --//
            CreateMap<NewChatDto, Chat>();
            CreateMap<Chat, ChatDetailsDto>();

            //-- mesage profiles --//
            CreateMap<PostNewMessageDto, Message>();
            CreateMap<PostMessageToChatDto, Message>();
            CreateMap<Message, MessageDetailsDto>();
        }
        
    }
}
