using MessengerAPI.Data.Models;
using System;

namespace MessengerAPI.Data.DataTransferObjects.Messages
{
    public class MessageDetailsDto
    {
        public string MessageId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string ApplicationUser { get; set; }
        public MessageType MessageType { get; set; }
    }

    public enum MessageType
    {
        Sent = 0,
        Received = 1
    }
}
