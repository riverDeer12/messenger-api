using AutoMapper;
using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.Models;

namespace MessengerAPI.Helpers
{
    public class AutoMapperProfiles : Profile 
    {
        public AutoMapperProfiles()
        {
            CreateMap<RegistrationDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDetailsDto>();
        }
        
    }
}
