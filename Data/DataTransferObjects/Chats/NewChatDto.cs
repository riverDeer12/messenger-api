using MessengerAPI.Data.DataTransferObjects.ApplicationUsers;
using MessengerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MessengerAPI.Data.DataTransferObjects.Chats
{
    public class NewChatDto
    {
        public List<UserDetailsDto> Users { get; set; }
        public string AdminId { get; set; }
        public string Name { get; set; }
        public bool Archived { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public ICollection<ApplicationUserChat> ApplicationUserChats { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }
        
        public NewChatDto()
        {
            Archived = false;
            SetChatTimestamps();
        }

        void SetChatTimestamps()
        {
            StartedAt = DateTime.Now;
            FinishedAt = DateTime.Now;
            LastActivityAt = DateTime.Now;
        }
    }
}
