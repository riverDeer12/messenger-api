using MessengerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Services.HelperClasses
{
    public class ChatResponse
    {
        public bool Success { get; set; }
        public Chat Chat { get; set; }
        public string ErrorMessage { get; set; }

        internal static ChatResponse Unsuccessful(string messages)
        {
            return new ChatResponse()
            {
                Success = false,
                ErrorMessage = messages
            };
        }

        internal static ChatResponse Successfull(Chat chat)
        {
            return new ChatResponse()
            {
                Success = true,
                Chat = chat
            };
        }
    }
}
