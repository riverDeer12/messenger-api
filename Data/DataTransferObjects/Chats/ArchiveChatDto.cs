using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Data.DataTransferObjects.Chats
{
    public class ArchiveChatDto
    {
        public Guid ChatId { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ArchiveChatDto()
        {
            UpdatedAt = DateTime.Now;
        }
    }
}
