using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helpers;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize]
    [RequireHttps]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper rHelp = new RoleHelper();
        private ProjectsHelper pHelp = new ProjectsHelper();

        [Authorize(Roles = "Admin, Project_Manager, DemoAdmin, DemoProject_Manager")]
        public ActionResult ManageUsers(int id)
        {
            #region
            //string currentPM = null;
            //foreach (var user in pHelp.UsersOnProject(id))
            //{
            //    if(roleHelper.IsUserInRole(user.Id, "Project_Manager"))
            //    {
            //        currentPM = user.Id;
            //    }
            //}
            //ViewBag.ProjectManagerId = new SelectList(roleHelper.UsersInRole("Project_Manager"), "Id", "Email", currentPM);

            //var projDevs = new List<string>();
            //foreach (var user in pHelp.UsersOnProject(id))
            //{
            //    if (roleHelper.IsUserInRole(user.Id, "Developer"))
            //    {
            //        projDevs.Add(user.Id);
            //    }
            //}
            //ViewBag.Developers = new MultiSelectList(roleHelper.UsersInRole("Developer"), "Id", "Email", projDevs);

            //var projSubs = new List<string>();
            //foreach (var user in pHelp.UsersOnProject(id))
            //{
            //    if (roleHelper.IsUserInRole(user.Id, "Submitter"))
            //    {
            //        projSubs.Add(user.Id);
            //    }
            //}
            //ViewBag.Submitters = new MultiSelectList(roleHelper.UsersInRole("Submitter"), "Id", "Email", projSubs);
            #endregion
            ViewBag.ProjectId = id;

            var AdminId = db.Users.ToList().Where(u => u.Id == User.Identity.GetUserId());
            if (User.IsInRole("Admin") || User.IsInRole("DemoAdmin"))
            {
                ViewBag.AdminId = new SelectList(AdminId, "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles(id, "Admin", "DemoAdmin").FirstOrDefault());
            }
            else
            {
                ViewBag.AdminId = new SelectList(rHelp.UsersIn2Roles("Admin", "DemoAdmin"), "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles(id, "Admin", "DemoAdmin").FirstOrDefault());
            }
            ViewBag.ProjectManagerId = new SelectList(rHelp.UsersIn2Roles("Project_Manager", "DemoProject_Manager"), "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles(id, "Project_Manager", "DemoProject_Manager").FirstOrDefault());
            ViewBag.Developers = new MultiSelectList(rHelp.UsersIn2Roles("Developer", "DemoDeveloper"), "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles(id, "Developer", "DemoDeveloper"));
            ViewBag.Submitters = new MultiSelectList(rHelp.UsersIn2Roles("Submitter", "DemoSubmitter"), "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles(id, "Submitter", "DemoSubmitter"));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUsers(int projectId, string adminId, string projectManagerId, List<string> developers, List<string> submitters)
        {
            foreach(var user in pHelp.UsersOnProject(projectId).ToList())
            {
                pHelp.RemoveUserFromProject(user.Id, projectId);
            }

            if (!string.IsNullOrEmpty(adminId))
            {
                pHelp.AddUserToProject(adminId, projectId);
            }

            if (!string.IsNullOrEmpty(projectManagerId))
            {
                pHelp.AddUserToProject(projectManagerId, projectId);
            }

            if(developers != null)            
            {
                foreach (var developerId in developers)
                {
                    pHelp.AddUserToProject(developerId, projectId);
                }
            }

            if (submitters != null)
            {
                foreach (var submitterId in submitters)
                {
                    pHelp.AddUserToProject(submitterId, projectId);
                }
            }

            return RedirectToAction("ManageUsers", new { id = projectId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageUsersDetails(int projectId, string adminId, string projectManagerId, List<string> developers, List<string> submitters)
        {
            foreach (var user in pHelp.UsersOnProject(projectId).ToList())
            {
                pHelp.RemoveUserFromProject(user.Id, projectId);
            }

            if (!string.IsNullOrEmpty(adminId))
            {
                pHelp.AddUserToProject(adminId, projectId);
            }

            if (!string.IsNullOrEmpty(projectManagerId))
            {
                pHelp.AddUserToProject(projectManagerId, projectId);
            }

            if (developers != null)
            {
                foreach (var developerId in developers)
                {
                    pHelp.AddUserToProject(developerId, projectId);
                }
            }

            if (submitters != null)
            {
                foreach (var submitterId in submitters)
                {
                    pHelp.AddUserToProject(submitterId, projectId);
                }
            }

            return RedirectToAction("Details", new { id = projectId });
        }

        // GET: Projects      
        public ActionResult Index()
        {
            //return View(db.Projects.ToList());       
            return View(pHelp.ListUserProjects(User.Identity.GetUserId()));
        }

        [Authorize(Roles = "Admin, Project_Manager, DemoAdmin, DemoProject_Manager")]
        public ActionResult AllProjectsIndex()
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            var ticket = db.Tickets.Where(t => t.ProjectId == project.Id).ToList();
            if (project == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProjectId = id;

            var AdminId = db.Users.ToList().Where(u => u.Id == User.Identity.GetUserId());
            if (User.IsInRole("Admin") || User.IsInRole("DemoAdmin"))
            {
                ViewBag.AdminId = new SelectList(AdminId, "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles((int)id, "Admin", "DemoAdmin").FirstOrDefault());
            }
            else
            {
                ViewBag.AdminId = new SelectList(rHelp.UsersIn2Roles("Admin", "DemoAdmin"), "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles((int)id, "Admin", "DemoAdmin").FirstOrDefault());
            }
            ViewBag.ProjectManagerId = new SelectList(rHelp.UsersIn2Roles("Project_Manager", "DemoProject_Manager"), "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles((int)id, "Project_Manager", "DemoProject_Manager").FirstOrDefault());
            ViewBag.Developers = new MultiSelectList(rHelp.UsersIn2Roles("Developer", "DemoDeveloper"), "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles((int)id, "Developer", "DemoDeveloper"));
            ViewBag.Submitters = new MultiSelectList(rHelp.UsersIn2Roles("Submitter", "DemoSubmitter"), "Id", "FullName", pHelp.ListUsersOnProjectIn2Roles((int)id, "Submitter", "DemoSubmitter"));
            var meh = new ProjectDetailsViewModel();
            meh.Projects = project;
            meh.TicketList = ticket;
            return View(meh);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, Project_Manager, DemoAdmin, DemoProject_Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Created,Updated")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTime.Now;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, Project_Manager, DemoAdmin, DemoProject_Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Created,Updated")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Updated = DateTime.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        //[Authorize(Roles = "Admin")]
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Project project = db.Projects.Find(id);
        //    if (project == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(project);
        //}

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
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
