using MessengerAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Data.DataTransferObjects.Messages
{
    public class PostMessageDataDto
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid ChatId { get; set; }
        public Guid UserId { get; set; }

        public PostMessageDataDto()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
