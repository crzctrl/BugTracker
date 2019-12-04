using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helpers;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize]
    [RequireHttps]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private RoleHelper roleHelper = new RoleHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();
        
        // GET:
        [Authorize(Roles = "Admin, DemoAdmin")]
        public ActionResult ManageRoles()
        {
            ViewBag.UserIds = new MultiSelectList(db.Users, "Id", "FullName");
            ViewBag.Role = new SelectList(db.Roles, "Name", "Name");
            var users = new List<ManageRolesViewModel>();
            foreach(var user in db.Users.ToList())
            {
                users.Add(new ManageRolesViewModel
                {
                    UserName = $"{user.LastName}, {user.FirstName}",
                    RoleName = roleHelper.ListUserRoles(user.Id).FirstOrDefault()
                });
            }
            return View(users);
        }

        // POST:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manageroles(List<string> userIds, string role)
        {
            if (userIds != null)
            {
                foreach (var userId in userIds)
                {
                    var userRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
                    if (userRole != null)
                    {
                        roleHelper.RemoveUserFromRole(userId, userRole);
                    }
                }

                if (!string.IsNullOrEmpty(role))
                {
                    foreach (var userId in userIds)
                    {
                        roleHelper.AddUserToRole(userId, role);
                    }
                }
            }

            return RedirectToAction("ManageRoles", "Admin");
        }

        [Authorize(Roles = "Admin, Project_Manager, DemoAdmin, DemoProject_Manager")]
        public ActionResult ManageProjectUsers()
        {
            var AdminId = db.Users.ToList().Where(u => u.Id == User.Identity.GetUserId());

            ViewBag.Projects = new MultiSelectList(db.Projects, "Id", "Name");
            //ViewBag.AdminId = new SelectList(roleHelper.UsersIn2Roles("Admin", "DemoAdmin"), "Id", "FullName");
            ViewBag.AdminId = new SelectList(AdminId, "Id", "FullName");
            ViewBag.ProjectManagerId = new SelectList(roleHelper.UsersIn2Roles("Project_Manager", "DemoProject_Manager"), "Id", "FullName");
            ViewBag.Developers = new MultiSelectList(roleHelper.UsersIn2Roles("Developer", "DemoDeveloper"), "Id", "FullName");
            ViewBag.Submitters = new MultiSelectList(roleHelper.UsersIn2Roles("Submitter", "DemoSubmitter"), "Id", "FullName");

            var myData = new List<UserProjectListViewModel>();
            UserProjectListViewModel userVM = null;

            foreach (var user in db.Users.ToList())
            {
                userVM = new UserProjectListViewModel
                {
                    Name = $"{user.FullName}",
                    ProjectNames = projHelper.ListUserProjects(user.Id).Select(p => p.Name).ToList()
                    //ProjectNames = projHelper.ListUserProjects(user.Id).Count() == 0 ? "N/A" : projHelper.ListUserProjects(user.Id).Select(p => p.Name).ToList()
                };

                if(userVM.ProjectNames.Count() == 0)
                {
                    userVM.ProjectNames.Add("N/A");
                }

                myData.Add(userVM);
            }

            return View(myData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageProjectUsers(List<int> projects, string adminId, string projectManagerId, List<string> developers, List<string> submitters)
        {
            if(projects != null)
            {
                foreach(var projectId in projects)
                {
                    foreach(var user in projHelper.UsersOnProject(projectId).ToList())
                    {
                        projHelper.RemoveUserFromProject(user.Id, projectId);
                    }

                    if (!string.IsNullOrEmpty(adminId))
                    {
                        projHelper.AddUserToProject(adminId, projectId);
                    }

                    if (!string.IsNullOrEmpty(projectManagerId))
                    {
                        projHelper.AddUserToProject(projectManagerId, projectId);
                    }

                    if(developers != null)
                    {
                        foreach(var developerId in developers)
                        {
                            projHelper.AddUserToProject(developerId, projectId);
                        }
                    }

                    if (submitters != null)
                    {
                        foreach (var submitterId in submitters)
                        {
                            projHelper.AddUserToProject(submitterId, projectId);
                        }
                    }
                }
            }
            
            return RedirectToAction("ManageProjectUsers");
        }
    }
}