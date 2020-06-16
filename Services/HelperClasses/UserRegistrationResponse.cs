using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;

namespace MessengerAPI.Services.HelperClasses
{
    public class UserRegistrationResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public List<string> ErrorMessage { get; set; }

        internal static UserRegistrationResponse Unsuccessful(IdentityResult result)
        {
            var messages = result.Errors.Select(x => x.Description).ToList();

            return new UserRegistrationResponse()
            {
                Success = false,
                ErrorMessage = messages
            };
        }

        internal static UserRegistrationResponse Successfull(string token)
        {
            return new UserRegistrationResponse()
            {
                Success = true,
                Token = token
            };
        }
        
    }
}
