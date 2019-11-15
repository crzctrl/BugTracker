using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketType
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }

        //Nav section
        public virtual ICollection<Ticket> Tickets { get; set; }

        public TicketType()
        {
            Tickets = new HashSet<Ticket>();
        }
    }
}