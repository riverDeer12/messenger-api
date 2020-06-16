using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MessengerAPI.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<ApplicationUserChat> ApplicationUserChats { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
