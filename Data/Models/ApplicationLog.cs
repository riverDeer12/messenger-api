using System;
using System.ComponentModel.DataAnnotations;

namespace MessengerAPI.Data.Models
{
    public class ApplicationLog
    {
        [Key]
        public Guid ApplicationLogId { get; set; }
        public string ErrorMessage { get; set; }
        public string Data { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}
