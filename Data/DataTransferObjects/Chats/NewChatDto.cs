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
        public string AdminId { get; set; }
        public ICollection<ApplicationUserChat> ApplicationUserChats { get; set; }
        public List<ApplicationUser> ApplicationUsers { get; set; }

        public NewChatDto(){}
        
        public NewChatDto(List<ApplicationUser> chatUsers)
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

        /// <summary>
        /// Set default chat name.
        /// If only 2 users then set name
        /// of user that is not creator.
        /// Creator is always first in list.
        /// </summary>
        void SetDefaultChatName()
        {
            var chatUsersNames = ApplicationUsers
                .Select(x => x.FirstName)
                .ToList();

            Name = ApplicationUsers.Count() == 2 ? 
                   ApplicationUsers[1].UserName :
                   string.Join(",", chatUsersNames);
        }
    }
}
