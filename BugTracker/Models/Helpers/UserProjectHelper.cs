using BugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class UserProjectHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ///<summary>Finds if user is assigned project</summary>
        public bool UserIsOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            return project.Users.Any(u => u.Id == userId);
        }

        public ApplicationUser ProjectAuthor(int projectId)
        {
            var project = db.Projects.Find(projectId);
            var author = project.Users.FirstOrDefault(u => u.Id == project.AuthorId);
            return author;
        }

        public Project UserProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            return user.Projects.FirstOrDefault(p => p.Id == projectId);
        }

        ///<summary>Adds user to project</summary>
        public void AddUserToProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            project.Users.Add(user);
            db.SaveChanges();
        }

        ///<summary>Removes user from project</summary>
        public void RemoveUserFromProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var user = db.Users.Find(userId);
            project.Users.Remove(user);
            db.SaveChanges();
        }
        ///<summary>Lists user projects</summary>
        public ICollection<Project> ListUserProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Projects.ToList();
        }

        /// <summary>Lists project users</summary>
        public ICollection<ApplicationUser> ListProjectUsers2(int projectId)
        {
            var project = db.Projects.Find(projectId);
            return project.Users.ToList();
        }
        ///<summary>Lists non project users</summary>
        public ICollection<ApplicationUser> ListNotProjectUsers(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }
    }
}