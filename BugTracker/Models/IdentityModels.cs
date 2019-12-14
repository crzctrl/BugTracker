using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        private RoleHelper rHelp = new RoleHelper();
        
        [Display(Name = "First Name")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Name must have min length of 1 and max Length of 25")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Name must have min length of 1 and max Length of 25")]
        public string LastName { get; set; }
        
        [Display(Name = "Display Name")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Name must have min length of 1 and max Length of 25")]
        public string DisplayName { get; set; }
        
        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName} ";
            }
        }

        //[NotMapped]
        //public string CorrectName
        //{
        //    get
        //    {
        //        return $"{LastName}, {FirstName}: {rHelp.ListUserRoles(Id).FirstOrDefault()}";
        //    }
        //}

        public string AvatarPath { get; set; }

        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        //public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual ICollection<Project> Projects { get; set; }

        public ApplicationUser()
        {
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketComments = new HashSet<TicketComment>();
            TicketHistories = new HashSet<TicketHistory>();
            //TicketNotifications = new HashSet<TicketNotification>();
            Projects = new HashSet<Project>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BugTracker.Models.Project> Projects { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.Ticket> Tickets { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketPriority> TicketPriorities { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketStatus> TicketStatuses { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketAttachment> TicketAttachments { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketComment> TicketComments { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketHistory> TicketHistories { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketNotification> TicketNotifications { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketType> TicketTypes { get; set; }
    }
}