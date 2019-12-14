using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }

        [StringLength(280, MinimumLength = 1, ErrorMessage = "Name must have min length of 1 and max Length of 280")]
        public string NotificationBody { get; set; }
        public bool isRead { get; set; }
        public DateTime Created { get; set; }

        //nav section
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        public virtual ApplicationUser Sender { get; set; }
    }
}