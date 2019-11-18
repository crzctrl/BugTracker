using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Helpers
{
    public class ProjectsHelper
    {
       private ApplicationDbContext db = new ApplicationDbContext();
       private RoleHelper rHelp = new RoleHelper();

        public List<string> ListUsersOnProjectInRole(int projectId, string roleName)
        {     
            var userIdList = new List<string>();
            foreach(var user in UsersOnProject(projectId))
            {
                if(rHelp.IsUserInRole(user.Id, roleName))
                {
                    userIdList.Add(user.Id);
                }
            }

            return userIdList;
        }

        public List<string> ListUsersOnProjectIn2Roles(int projectId, string roleName1, string roleName2)
        {
            var userIdList = new List<string>();
            foreach (var user in UsersOnProject(projectId))
            {
                if (rHelp.IsUserInRole(user.Id, roleName1) || rHelp.IsUserInRole(user.Id, roleName2))
                {
                    userIdList.Add(user.Id);
                }
            }

            return userIdList;
        }

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var flag = project.Users.Any(u => u.Id == userId);
            return (flag);
        }

        public ICollection<Project> ListUserProjects(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);

            var projects = user.Projects.ToList();
            return (projects);
        }

        public void AddUserToProject(string userId, int projectId)
        {
            if (!IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);

                proj.Users.Add(newUser);
                db.SaveChanges();
            }
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                Project proj = db.Projects.Find(projectId);
                var delUser = db.Users.Find(userId);

                proj.Users.Remove(delUser);
                db.Entry(proj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ICollection<ApplicationUser> UsersOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }

        public ICollection<ApplicationUser> UsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }


    }
}