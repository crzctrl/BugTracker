using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must have min length of 1 and max Length of 50")]
        public string Name { get; set; }
        [StringLength(280, MinimumLength = 1, ErrorMessage = "Name must have min length of 1 and max Length of 280")]
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        //Nav section
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }


        public Project()
        {
            Tickets = new HashSet<Ticket>();
            Users = new HashSet<ApplicationUser>();
        }
    }

    public class ProjectDetailsViewModel
    {
        //public virtual ICollection<Project> Projects { get; set; }
        //public virtual ICollection<Ticket> Tickets { get; set; }

        //public ProjectDetailsViewModel()
        //{
        //    Projects = new HashSet<Project>();
        //    Tickets = new HashSet<Ticket>();
        //}

        public Project Projects { get; set; }
        public Ticket Tickets { get; set; }
        public virtual ICollection<Ticket> TicketList { get; set; }

        public ProjectDetailsViewModel()
        {
            Projects = new Project();
            Tickets = new Ticket();
            TicketList = new HashSet<Ticket>();
        }

    }
}