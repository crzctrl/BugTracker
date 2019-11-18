﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Helpers
{
    public class NotificationHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        private TicketHistoryHelper hHelp = new TicketHistoryHelper();
        
        public void ManageNotifications(Ticket oldTicket, Ticket newTicket)
        {
            var assignedTicket = oldTicket.DeveloperId == null && newTicket.DeveloperId != null;
            var unassignedTicket = oldTicket.DeveloperId != null && newTicket.DeveloperId == null;
            var reassignedTicket = oldTicket.DeveloperId != null && newTicket.DeveloperId != null && oldTicket.DeveloperId != newTicket.DeveloperId;
            var statusTicket = oldTicket.TicketStatusId != newTicket.TicketStatusId;
            var priorityTicket = oldTicket.TicketPriorityId != newTicket.TicketPriorityId;

            if (assignedTicket)
                AddAssignmentNotification(oldTicket, newTicket);
            else if (unassignedTicket)
                AddUnassignmentNotification(oldTicket, newTicket);
            else if (reassignedTicket)
            {
                AddAssignmentNotification(oldTicket, newTicket);
                AddUnassignmentNotification(oldTicket, newTicket);
            }
            else if (statusTicket)
            {
                AddStatusNotification(oldTicket, newTicket);
            }
            else if (priorityTicket)
            {
                AddPriorityNotification(oldTicket, newTicket);
            }
        }

        private void AddAssignmentNotification(Ticket oldTicket, Ticket newTicket)
        {
            var notification = new TicketNotification
            {
                TicketId = newTicket.Id,
                isRead = false,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                RecipientId = newTicket.DeveloperId,
                Created = DateTime.Now,
                NotificationBody = $"You've been assigned to Ticket #{newTicket.Id}: {newTicket.Title} from Project: {newTicket.Project.Name}."
            };

            db.TicketNotifications.Add(notification);
            db.SaveChanges();
        }

        private void AddUnassignmentNotification(Ticket oldTicket, Ticket newTicket)
        {
            var notification = new TicketNotification
            {
                TicketId = newTicket.Id,
                isRead = false,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                RecipientId = oldTicket.DeveloperId,
                Created = DateTime.Now,
                NotificationBody = $"You've been unassigned from Ticket #{newTicket.Id}: {newTicket.Title}."
            };

            db.TicketNotifications.Add(notification);
            db.SaveChanges();
        }

        private void AddStatusNotification(Ticket oldTicket, Ticket newTicket)
        {
            var notification = new TicketNotification
            {
                TicketId = newTicket.Id,
                isRead = false,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                RecipientId = oldTicket.DeveloperId,
                Created = DateTime.Now,
                NotificationBody = $"Ticket #{newTicket.Id}: {newTicket.Title} status was changed to: {newTicket.TicketStatus.StatusName}."
            };

            db.TicketNotifications.Add(notification);
            db.SaveChanges();
        }

        private void AddPriorityNotification(Ticket oldTicket, Ticket newTicket)
        {
            var notification = new TicketNotification
            {
                TicketId = newTicket.Id,
                isRead = false,
                SenderId = HttpContext.Current.User.Identity.GetUserId(),
                RecipientId = oldTicket.DeveloperId,
                Created = DateTime.Now,
                NotificationBody = $"Ticket #{newTicket.Id}: {newTicket.Title} priority was changed to: {newTicket.TicketPriority.PriorityName}."
            };

            db.TicketNotifications.Add(notification);
            db.SaveChanges();
        }

        public void AddCommentNotification(TicketComment ticket)
        {
            var title = db.Tickets.Where(t => t.Id == ticket.TicketId).FirstOrDefault().Title;

            var notification = new TicketNotification
            {
                TicketId = ticket.TicketId,
                isRead = false,
                SenderId = ticket.UserId,
                RecipientId = db.Tickets.Where(t => t.Id == ticket.TicketId).FirstOrDefault().DeveloperId,
                Created = DateTime.Now,
                //NotificationBody = $"{ticket.User.DisplayName} posted a comment was added to Ticket: #{ticket.TicketId}: {title}."
                NotificationBody = $"A comment was added to Ticket: #{ticket.TicketId}: {title}."
            };

            db.TicketNotifications.Add(notification);
            db.SaveChanges();
        }

        public void AddAttachmentNotification(TicketAttachment ticket)
        {
            var title = db.Tickets.Where(t => t.Id == ticket.TicketId).FirstOrDefault().Title;

            var notification = new TicketNotification
            {
                TicketId = ticket.TicketId,
                isRead = false,
                SenderId = ticket.UserId,
                RecipientId = db.Tickets.Where(t => t.Id == ticket.TicketId).FirstOrDefault().DeveloperId,
                Created = DateTime.Now,
                //NotificationBody = $"{ticket.User.DisplayName} added an attachment was added to Ticket #{ticket.TicketId}: {title}."
                NotificationBody = $"An attachment was added to Ticket #{ticket.TicketId}: {title}."
            };

            db.TicketNotifications.Add(notification);
            db.SaveChanges();
        }

        public static List<TicketNotification> GetUnreadNotifications()
        {
            var currentUserId = HttpContext.Current.User.Identity.GetUserId();

            return db.TicketNotifications.Include("Sender").Include("Recipient").Where(t => t.RecipientId == currentUserId && !t.isRead).ToList();
        }
    }
}