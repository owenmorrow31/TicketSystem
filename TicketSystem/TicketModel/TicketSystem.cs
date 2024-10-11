using System;
using System.Collections.Generic;

namespace TicketSystem.Models
{
    public class Ticket
    {
        public string TicketID { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // e.g., "Open", "Closed"
        public string Priority { get; set; } // e.g., "High", "Low"
        public DateTime DateCreated { get; set; }
        public DateTime LastUpdated { get; set; }

        // Relations
        public string CreatedByUserId { get; set; }
        public string AssignedToUserId { get; set; }
        public List<Comment> Comments { get; set; }
    }
}