using MessengerAPI.Data.DataTransferObjects.Messages;
using MessengerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Services.HelperClasses
{
    public class MessageResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public MessageDetailsDto  Message { get; set; }

        internal static MessageResponse Unsuccessful(string messages)
        {
            return new MessageResponse()
            {
                Success = false,
                ErrorMessage = messages
            };
        }

        internal static MessageResponse Successfull(MessageDetailsDto message)
        {
            return new MessageResponse()
            {
                Success = true,
                Message = message
            };
        }
    }
}
