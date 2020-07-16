using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.DataTransferObjects.Messages;
using MessengerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Data.DataTransferObjects.Chats
{
    public class ChatDetailsDto
    {
        public string ChatId { get; set; }
        public string Name { get; set; }
        public DateTime LastActivityAt { get; set; }
        public List<UserDetailsDto> Users { get; set; }
        public List<MessageDetailsDto> Messages { get; set; }

        internal void SetChatName()
        {
            if (!string.IsNullOrEmpty(Name)) return;

            if(Users.Count == 1)
            {
                Name = Users[0].UserName;
            }
            else
            {
                Name = Users
                    .Select(x => x.UserName)
                    .ToString();
            }
        }
    }
}
