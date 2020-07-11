using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Services.ResponseClasses
{
    public class UserResponse
    {
        public bool Success { get; set; }
        public UserDetailsDto User { get; set; }
        public List<UserDetailsDto> Users { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        internal static UserResponse Unsuccessful(string message)
        {
            return new UserResponse
            {
                Success = false,
                ErrorMessage = message 
            };
        }
        internal static UserResponse Successful()
        {
            return new UserResponse
            {
                Success = true
            };
        }

        internal static UserResponse Successful(UserDetailsDto user)
        {
            return new UserResponse
            {
                Success = true,
                User = user
            };
        }

        internal static UserResponse Successful(List<UserDetailsDto> users)
        {
            return new UserResponse
            {
                Success = true,
                Users = users
            };
        }
    }
}
