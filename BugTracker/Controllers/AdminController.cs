using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : ApplicationBaseController
    {
        //ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            List<AdminViewModels> users = new List<AdminViewModels>();
            UserRoleHelper helper = new UserRoleHelper();
            foreach (var user in db.Users.ToList())
            {
                var eachUser = new AdminViewModels();
                eachUser.User = user;
                eachUser.SelectedRoles = helper.ListUserRoles(user.Id).ToArray();
                users.Add(eachUser);
            }
            return View(users.OrderBy(u => u.User.LastName).ToList());
        }

        //get EditUserRoles
        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            var helper = new UserRoleHelper();
            var model = new AdminViewModels();
            model.User = user;
            model.SelectedRoles = helper.ListUserRoles(id).ToArray();
            model.Roles = new MultiSelectList(db.Roles, "Name", "Name", model.SelectedRoles);
            return View(model);
        }

        //post EditUserRoles
        [HttpPost]
        public ActionResult EditUserRoles(AdminViewModels model)
        {
            var user = db.Users.Find(model.User.Id);
            UserRoleHelper helper = new UserRoleHelper();
            foreach (var role in db.Roles.Select(r => r.Name).ToList())
            {
                helper.RemoveUserFromRole(user.Id, role);
            }
            if (model.SelectedRoles != null)
            {
                foreach (var role in model.SelectedRoles)
                {
                    helper.AddUserToRole(user.Id, role);
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}