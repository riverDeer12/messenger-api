using MessengerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MessengerAPI.Data.DataTransferObjects.Chats
{
    public class NewChatDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public List<Guid> UserIds { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public List<ApplicationUser> ChatUsers { get; set; }
        public List<Message> Messages { get; set; }

        public NewChatDto(){}
        
        public NewChatDto(List<ApplicationUser> chatUsers, Message message)
        {
            ChatUsers = chatUsers;
            Messages.Add(message);
            SetDefaultChatName();
            SetChatTimestamps();
        }

        void SetChatTimestamps()
        {
            StartedAt = DateTime.Now;
            FinishedAt = DateTime.Now;
            LastActivityAt = DateTime.Now;
        }

        void SetDefaultChatName()
        {
            var chatUsersNames = ChatUsers
                .Select(x => x.FirstName)
                .ToList();

            Name = string.Join(",", chatUsersNames);
        }
    }
}
