using MessengerAPI.Data.DataTransferObjects.Chats;
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
        public List<Chat> Chats { get; set; }
        public ChatDetailsDto ChatDetails{get; set;}
        public string ErrorMessage { get; set; }

        internal static ChatResponse Unsuccessful(string messages)
        {
            return new ChatResponse()
            {
                Success = false,
                ErrorMessage = messages
            };
        }

        internal static ChatResponse Successfull()
        {
            return new ChatResponse()
            {
                Success = true
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

        internal static ChatResponse Successfull(List<Chat> chats)
        {
            return new ChatResponse()
            {
                Success = true,
                Chats = chats
            };
        }

        internal static ChatResponse Successfull(ChatDetailsDto chatDetails)
        {
            return new ChatResponse()
            {
                Success = true,
                ChatDetails = chatDetails
            };
        }
    }
}
