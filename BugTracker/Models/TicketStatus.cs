using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }

        //Nav section
        public virtual ICollection<Ticket> Tickets { get; set; }

        public TicketStatus()
        {
            Tickets = new HashSet<Ticket>();
        }
    }
}