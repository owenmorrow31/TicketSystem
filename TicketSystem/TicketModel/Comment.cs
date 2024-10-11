using System;

namespace TicketSystem.Models
{
    public class Comment
    {
        public string CommentID { get; set; }
        public string TicketID { get; set; }
        public string UserID { get; set; }
        public string CommentText { get; set; }
        public DateTime DateCreated { get; set; }
    }
}