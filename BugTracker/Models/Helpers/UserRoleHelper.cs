using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class UserRoleHelper
    {
        //Instantiate UserManager
        private UserManager<ApplicationUser> userManager =
            new UserManager<ApplicationUser>(new UserStore<ApplicationUser>
                (new ApplicationDbContext()));
        //Instantiate ApplicationDbContext
        private ApplicationDbContext db = new ApplicationDbContext();

        //List all users
        public ICollection<ApplicationUser> ListUsers()
        {
            List<ApplicationUser> List = userManager.Users.ToList();
            return List;
        }

        //check if user is in specific role
        public bool UserIsInRole(string userId, string roleName)
        {
            return userManager.IsInRole(userId, roleName);
        }

        //lists all roles a user has
        public ICollection<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);
        }

        //add user to role
        public bool AddUserToRole(string userId, string roleName)
        {
            var result = userManager.AddToRole(userId, roleName);
            return result.Succeeded;
        }

        //remove user from role
        public bool RemoveUserFromRole(string userId, string roleName)
        {
            var result = userManager.RemoveFromRole(userId, roleName);
            return result.Succeeded;
        }

        //list all users in role
        public ICollection<ApplicationUser> ListRoleUsers(string roleName)
        {
            return db.Users.Where(u => u.Roles.Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r).Any(r => r.Name.Equals(roleName))).ToList();
        }

        //list all users in role by given users list
        public ICollection<ApplicationUser> ListRoleUsers(ICollection<ApplicationUser> users, string roleName)
        {
            return users.Where(u => u.Roles.Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r).Any(r => r.Name.Equals(roleName))).ToList();
        }

        //list users not in role
        public ICollection<ApplicationUser> ListNotRoleUsers(string roleName)
        {
            List<ApplicationUser> resultList = new List<ApplicationUser>();
            List<ApplicationUser> List = userManager.Users.ToList();
            foreach (var user in List)
            {
                if (!UserIsInRole(user.Id, roleName))
                {
                    resultList.Add(user);
                }
            }
            return resultList;
        }
    }
}