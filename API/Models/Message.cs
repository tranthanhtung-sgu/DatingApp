using System;

namespace API.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public AppUser Sender { get; set; } 
        public string SenderUsername { get; set; }
        public int RecipientId { get; set; }
        public AppUser Recipient { get; set; }
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime DateSent { get; set; } = DateTime.Now;
    }
}