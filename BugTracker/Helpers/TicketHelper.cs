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

        public List<Ticket> ListMyProjectTickets()
        {
            var myTickets = new List<Ticket>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            myTickets.AddRange(user.Projects.SelectMany(p => p.Tickets));
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
                    //myComments.AddRange(db.TicketComments.Where(t => t.UserId == userId));
                    myComments.AddRange(user.Projects.SelectMany(p => p.Tickets).SelectMany(t => t.TicketComments));
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

        public List<TicketAttachment> ListMyAttachments()
        {
            var myAttach = new List<TicketAttachment>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myRole = rHelp.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Admin":
                case "DemoAdmin":
                    myAttach.AddRange(db.TicketAttachments);
                    break;
                case "Project_Manager":
                case "DemoProject_Manager":
                    //myAttach.AddRange(db.TicketAttachments.Where(t => t.UserId == userId));
                    myAttach.AddRange(user.Projects.SelectMany(p => p.Tickets).SelectMany(p => p.TicketAttachments));
                    break;
                case "Developer":
                case "DemoDeveloper":
                    myAttach.AddRange(db.TicketAttachments.Where(t => t.UserId == userId));
                    break;
                case "Submitter":
                case "DemoSubmitter":
                    myAttach.AddRange(db.TicketAttachments.Where(t => t.UserId == userId));
                    break;
            }

            return myAttach;
        }

        public List<TicketHistory> ListMyHistory()
        {
            var myHist = new List<TicketHistory>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myRole = rHelp.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                case "Admin":
                case "DemoAdmin":
                    myHist.AddRange(db.TicketHistories);
                    break;
                case "Project_Manager":
                case "DemoProject_Manager":
                case "Developer":
                case "DemoDeveloper":
                case "Submitter":
                case "DemoSubmitter":
                    //myHist.AddRange(db.TicketHistories.Where(t => t.UserId == userId));
                    myHist.AddRange(user.Projects.SelectMany(p => p.Tickets).SelectMany(p => p.TicketHistories));
                    break;
                //case "Developer":
                //case "DemoDeveloper":
                //    myHist.AddRange(db.TicketHistories.Where(t => t.UserId == userId));
                //    break;
                //case "Submitter":
                //case "DemoSubmitter":
                //    myHist.AddRange(db.TicketHistories.Where(t => t.UserId == userId));
                //    break;
            }

            return myHist;
        }

        public List<TicketNotification> ListMyNotifications()
        {
            var myNote = new List<TicketNotification>();
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myRole = rHelp.ListUserRoles(userId).FirstOrDefault();

            switch (myRole)
            {
                //case "Admin":
                //case "DemoAdmin":
                //    myNote.AddRange(db.TicketNotifications);
                //    break;
                //case "Project_Manager":
                //case "DemoProject_Manager":
                //    myNote.AddRange(user.Projects.SelectMany(p => p.Tickets).SelectMany(p => p.TicketNotifications));
                //    break;
                case "Developer":
                case "DemoDeveloper":
                    myNote.AddRange(db.TicketNotifications.Where(t => t.RecipientId == userId));
                    break;
            }

            return myNote;
        }
    }
}