using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Helpers
{
    public class SecurityHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper rHelp = new RoleHelper();
        private ProjectsHelper pHelp = new ProjectsHelper();
        
        //public ActionResult IsUserOnTicket(int? id)
        //{
        //    Ticket ticket = db.Tickets.Find(id);
        //    var user = HttpContext.Current.User.Identity.GetUserId();
            
        //    var pm = rHelp.IsUserInRole(user, "Project_Manager") || rHelp.IsUserInRole(user, "DemoProject_Manager");
        //    if (pm)
        //    {
        //        if (!pHelp.IsUserOnProject(user, ticket.ProjectId))
        //        {
        //            return View("Error");
        //        }
        //    }
        //}
    }
}