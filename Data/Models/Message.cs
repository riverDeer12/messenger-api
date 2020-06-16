using System;
using System.ComponentModel.DataAnnotations;

namespace MessengerAPI.Data.Models
{
    public class Message
    {
        [Key]
        public Guid MessageId { get; set; }
        public string Content { get; set; }
        public bool Archived { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        public Guid UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
