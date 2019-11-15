using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Helpers
{
    public class TicketHistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void RecordHistoricalChanges(Ticket oldTicket, Ticket newTicket)
        {
            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    Property = "TicketStatusId",
                    OldValue = oldTicket.TicketStatus.StatusName,
                    NewValue = newTicket.TicketStatus.StatusName,
                    Changed = (DateTime)newTicket.Updated,
                    TicketId = newTicket.Id,
                    UserId = HttpContext.Current.User.Identity.GetUserId()
                };

                db.TicketHistories.Add(newHistoryRecord);
            }

            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    Property = "TicketStatusId",
                    OldValue = oldTicket.TicketPriority.PriorityName,
                    NewValue = newTicket.TicketPriority.PriorityName,
                    Changed = (DateTime)newTicket.Updated,
                    TicketId = newTicket.Id,
                    UserId = HttpContext.Current.User.Identity.GetUserId()
                };

                db.TicketHistories.Add(newHistoryRecord);
            }

            if (oldTicket.DeveloperId != newTicket.DeveloperId)
            {
                var newHistoryRecord = new TicketHistory
                {
                    Property = "DeveloperId",
                    OldValue = oldTicket.Developer == null ? "Unassigned" : oldTicket.Developer.Email,
                    NewValue = newTicket.Developer == null ? "Unassigned" : newTicket.Developer.Email,
                    Changed = (DateTime)newTicket.Updated,
                    TicketId = newTicket.Id,
                    UserId = HttpContext.Current.User.Identity.GetUserId()
                };

                db.TicketHistories.Add(newHistoryRecord);
            }

            db.SaveChanges();
        }
    }
}