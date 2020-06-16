using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Data.Models
{
    public class ApplicationUserChat
    {
        public string UserId { get; set; }
        public Guid ChatId { get; set; }
        public bool Archived { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Chat Chat { get; set; }
    }
}
