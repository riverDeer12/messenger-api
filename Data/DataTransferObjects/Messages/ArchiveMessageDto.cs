using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Data.DataTransferObjects.Messages
{
    public class ArchiveMessageDto
    {
        public Guid MessageId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Archived { get; set; }

        public ArchiveMessageDto()
        {
            Archived = true;
            UpdatedAt = DateTime.Now;
        }
    }
}
