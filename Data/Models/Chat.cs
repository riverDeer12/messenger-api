using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MessengerAPI.Data.Models
{
    public class Chat
    {
        [Key]
        public Guid ChatId { get; set; }
        public string Name { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<ApplicationUserChat> ApplicationUserChats { get; set; }
    }
}
