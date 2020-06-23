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
        public List<Guid> UserIds { get; set; }
        public string Name { get; set; }
        public bool Archived { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public ICollection<ApplicationUserChat> ApplicationUserChats { get; set; }
        public ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public NewChatDto(){}
        
        public NewChatDto(ICollection<ApplicationUser> chatUsers)
        {
            ApplicationUsers = chatUsers;
            Archived = true;
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
            var chatUsersNames = ApplicationUsers
                .Select(x => x.FirstName)
                .ToList();

            Name = string.Join(",", chatUsersNames);
        }
    }
}
