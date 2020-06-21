using System;
using System.Collections.Generic;

namespace MessengerAPI.Data.DataTransferObjects.Messages
{
    public class PostNewMessageDto
    {
        public string UserId { get; set; }
        public List<string> ReceiverIds { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public PostNewMessageDto()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
