using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class GraphingController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public JsonResult TicketPriorityChartData()
        {
            var myData = new List<MorrisBarData>();
            MorrisBarData data = null;
            foreach(var priority in db.TicketPriorities.ToList())
            {
                data = new MorrisBarData();
                data.label = priority.PriorityName;
                data.value = db.Tickets.Where(t => t.TicketPriority.PriorityName == priority.PriorityName).Count();
                myData.Add(data);
            }

            return Json(myData);
        }

        public JsonResult PMTicketPriorityChartData()
        {
            var myData = new List<MorrisBarData>();
            MorrisBarData data = null;
            var user = User.Identity.GetUserId();
            foreach (var priority in db.TicketPriorities.ToList())
            {
                data = new MorrisBarData();
                data.label = priority.PriorityName;
                data.value = db.Tickets.Where(t => t.TicketPriority.PriorityName == priority.PriorityName).Count();
                myData.Add(data);
            }

            return Json(myData);
        }

        public JsonResult TicketStatusChartData()
        {
            var myData = new List<MorrisBarData>();
            foreach (var status in db.TicketStatuses.ToList())
            {
                myData.Add(new MorrisBarData
                {
                    label = status.StatusName,
                    value = db.Tickets.Where(t => t.TicketStatus.StatusName == status.StatusName).Count()
                });
            }            

            return Json(myData);
        }

        public JsonResult TicketTypeChartData()
        {
            var myData = new List<MorrisBarData>();
            foreach (var type in db.TicketTypes.ToList())
            {
                myData.Add(new MorrisBarData
                {
                    label = type.TypeName,
                    value = db.Tickets.Where(t => t.TicketType.TypeName == type.TypeName).Count()
                });
            }

            return Json(myData);
        }
    }
}