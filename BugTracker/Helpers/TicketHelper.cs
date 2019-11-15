using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Helpers
{
    public class TicketHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper rHelp = new RoleHelper();

        public int SetDefaultTicketStatus()
        {
            return db.TicketStatuses.FirstOrDefault(ts => ts.StatusName == "Open").Id;
        }

        public List<Ticket> ListMyTickets()
        {
            var myTickets = new List<Ticket>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myRole = rHelp.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Admin":
                case "DemoAdmin":
                    myTickets.AddRange(db.Tickets);
                    break;
                case "Project_Manager":
                case "DemoProject_Manager":
                    myTickets.AddRange(user.Projects.SelectMany(p => p.Tickets));
                    break;
                case "Developer":
                case "DemoDeveloper":
                    myTickets.AddRange(db.Tickets.Where(t => t.DeveloperId == userId));
                    break;
                case "Submitter":
                case "DemoSubmitter":
                    myTickets.AddRange(db.Tickets.Where(t => t.SubmitterId == userId));
                    break;
            }

            return myTickets;
        }

        public List<TicketComment> ListMyComments()
        {
            var myComments = new List<TicketComment>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myRole = rHelp.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Admin":
                case "DemoAdmin":
                    myComments.AddRange(db.TicketComments);
                    break;
                case "Project_Manager":
                case "DemoProject_Manager":
                    myComments.AddRange(db.TicketComments.Where(t => t.UserId == userId));
                    break;
                case "Developer":
                case "DemoDeveloper":
                    myComments.AddRange(db.TicketComments.Where(t => t.UserId == userId));
                    break;
                case "Submitter":
                case "DemoSubmitter":
                    myComments.AddRange(db.TicketComments.Where(t => t.UserId == userId));
                    break;
            }

            return myComments;
        }
    }
}