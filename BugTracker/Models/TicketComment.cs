using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        
        [Required(ErrorMessage = "Name must have min length of 1 and max Length of 500")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Name must have min length of 1 and max Length of 500")]
        public string CommentBody { get; set; }
        public DateTime Created { get; set; }

        //nav section
        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}