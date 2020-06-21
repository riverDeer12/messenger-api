using AutoMapper;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.DataTransferObjects.Chats;
using MessengerAPI.Data.Models;

namespace MessengerAPI.Helpers
{
    public class AutoMapperProfiles : Profile 
    {
        public AutoMapperProfiles()
        {
            //-- application user profiles --//
            CreateMap<RegistrationDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDetailsDto>();

            //-- chat profiles --//
            CreateMap<NewChatDto, Chat>();
        }
        
    }
}
