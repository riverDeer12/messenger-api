using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MessengerAPI.Data.DataTransferObjects.Chats
{
    public class PostChatDataDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public List<Guid> UserIds { get; set; }
        public Guid ChatId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        
        public PostChatDataDto()
        {
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
