using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [Authorize]
    [RequireHttps]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUser user = new ApplicationUser();
        private RoleHelper rHelp = new RoleHelper();
        private TicketHelper tHelp = new TicketHelper();
        private TicketHistoryHelper hHelp = new TicketHistoryHelper();
        private NotificationHelper nHelp = new NotificationHelper();
        private ProjectsHelper pHelp = new ProjectsHelper();

        // GET: Tickets
        public ActionResult Index()
        {
            return View(tHelp.ListMyTickets());
        }

        [Authorize(Roles = "Developer, DemoDeveloper")]
        public ActionResult ProjectTicketsIndex()
        {
            var myTickets = new List<Ticket>();
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var myRole = rHelp.ListUserRoles(userId).FirstOrDefault();

            if(myRole == "Developer" || myRole == "DemoDeveloper")
            {
                myTickets.AddRange(user.Projects.SelectMany(p => p.Tickets));
            }

            return View(myTickets);
        }

        [Authorize(Roles = "Admin, Project_Manager, Developer, Submitter, DemoAdmin, DemoProject_Manager, DemoDeveloper, DemoSubmitter")]
        public ActionResult AllTicketsIndex()
        {
            var tickets = db.Tickets.Include(t => t.Developer).Include(t => t.Project).Include(t => t.Submitter).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, Project_Manager, Developer, Submitter, DemoAdmin, DemoProject_Manager, DemoDeveloper, DemoSubmitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var sub = rHelp.IsUserInRole(User.Identity.GetUserId(), "Submitter") || rHelp.IsUserInRole(User.Identity.GetUserId(), "DemoSubmitter");
            if (sub)
            {
                if (ticket.SubmitterId != User.Identity.GetUserId())
                {
                    return HttpNotFound();
                }
            }
            var dev = rHelp.IsUserInRole(User.Identity.GetUserId(), "Developer") || rHelp.IsUserInRole(User.Identity.GetUserId(), "DemoDeveloper");
            if (dev)
            {
                if (ticket.DeveloperId != User.Identity.GetUserId())
                {
                    return HttpNotFound();
                }
            }
            var pm = rHelp.IsUserInRole(User.Identity.GetUserId(), "Project_Manager") || rHelp.IsUserInRole(User.Identity.GetUserId(), "DemoProject_Manager");
            if (pm)
            {
                if (!pHelp.IsUserOnProject(User.Identity.GetUserId(), ticket.ProjectId))
                {
                    return HttpNotFound();
                }
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter, DemoSubmitter")]
        public ActionResult Create()
        {
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FullName");
            ViewBag.ProjectId = new SelectList(pHelp.ListUserProjects(User.Identity.GetUserId()), "Id", "Name");
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FullName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "PriorityName");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "StatusName");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "TypeName");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectId,TicketTypeId,TicketPriorityId,Title,Description")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = DateTime.Now;
                ticket.SubmitterId = User.Identity.GetUserId();
                ticket.TicketStatusId = tHelp.SetDefaultTicketStatus();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FullName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            //ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FullName", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "PriorityName", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "StatusName", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "TypeName", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, Project_Manager, Developer, Submitter, DemoAdmin, DemoProject_Manager, DemoDeveloper, DemoSubmitter")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var sub = rHelp.IsUserInRole(User.Identity.GetUserId(), "Submitter") || rHelp.IsUserInRole(User.Identity.GetUserId(), "DemoSubmitter");
            if (sub)
            {
                if (ticket.SubmitterId != User.Identity.GetUserId())
                {
                    return HttpNotFound();
                }
            }
            var dev = rHelp.IsUserInRole(User.Identity.GetUserId(), "Developer") || rHelp.IsUserInRole(User.Identity.GetUserId(), "DemoDeveloper");
            if (dev)
            {
                if (ticket.DeveloperId != User.Identity.GetUserId())
                {
                    return HttpNotFound();
                }
            }
            //db.Projects.Find(ticketid)
            var pm = rHelp.IsUserInRole(User.Identity.GetUserId(), "Project_Manager") || rHelp.IsUserInRole(User.Identity.GetUserId(), "DemoProject_Manager");
            if (pm)
            {
                if (!pHelp.IsUserOnProject(User.Identity.GetUserId(), ticket.ProjectId))
                {
                    return HttpNotFound();
                }
            }
            ViewBag.DeveloperId = new SelectList(rHelp.UsersIn2Roles("Developer", "DemoDeveloper"), "Id", "FullName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(rHelp.UsersIn2Roles("Submitter", "DemoSubmitter"), "Id", "FullName", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "PriorityName", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "StatusName", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "TypeName", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,SubmitterId,DeveloperId,Title,Description,Created")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                ticket.Updated = DateTime.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                var newTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                hHelp.RecordHistoricalChanges(oldTicket, newTicket);
                nHelp.ManageNotifications(oldTicket, newTicket);
                await AssignTicket(ticket);

                return RedirectToAction("Index");
            }
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FullName", ticket.DeveloperId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            //ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FullName", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "PriorityName", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "StatusName", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "TypeName", ticket.TicketTypeId);

            return View(ticket);
        }

        // GET: Tickets/Delete/5
        //[Authorize(Roles ="Admin")]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ticket ticket = db.Tickets.Find(id);
        //    if (ticket == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticket);
        //}

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //public ActionResult AssignTicket(int? id)
        //{
        //    RoleHelper rHelp = new RoleHelper();
        //    var ticket = db.Tickets.Find(id);
        //    var users = rHelp.UsersIn2Roles("Developer", "DemoDeveloper").ToList();
        //    ViewBag.DeveloperId = new SelectList(users, "Id", "FullName", ticket.DeveloperId);

        //    return View(ticket);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignTicket(Ticket model)
        {
            var ticket = db.Tickets.Find(model.Id);
            ticket.DeveloperId = model.DeveloperId;
            //if (ticket.DeveloperId == null)
            //{
            //    ticket.TicketStatusId = db.TicketStatuses.Where(t => t.StatusName == "Open").FirstOrDefault().Id;
            //}
            //else
            //{
            //    ticket.TicketStatusId = db.TicketStatuses.Where(t => t.StatusName == "Assigned").FirstOrDefault().Id;
            //}

            db.SaveChanges();

            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);

            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = db.Users.Find(model.DeveloperId);

                msg.Body = $"You have been assigned a new Ticket.<br/>Please click the following link to view the details: <a href=\"{callbackUrl}\">new ticket</a>";
                msg.Destination = user.Email;
                msg.Subject = "Alert: Assignment";

                await ems.SendMailAsync(msg);
            }
            catch
            {
                await Task.FromResult(0);
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
