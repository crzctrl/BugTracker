using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }

        public string Property { get; set; } //type
        public string OldValue { get; set; } //type
        public string NewValue { get; set; } //type
        public DateTime Changed { get; set; } //type

        //nav section
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}