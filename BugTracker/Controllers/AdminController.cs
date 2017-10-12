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
            ViewBag.Active = "admin";
            UserRoleHelper helper = new UserRoleHelper();
            AdminIndexViewModel vm = new AdminIndexViewModel();
            vm.Users = new HashSet<AdminUserRoles>();
            foreach (var user in db.Users.ToList())
            {
                AdminUserRoles loopVm = new AdminUserRoles();
                loopVm.User = user;
                loopVm.UserRoles = helper.ListUserRoles(user.Id).ToArray();
                vm.Users.Add(loopVm);
            }
            vm.Users = vm.Users.OrderBy(u => u.User.LastName).ToList();
            return View(vm);
        }

        //get EditUserRoles
        public ActionResult EditUserRoles(string id)
        {
            var user = db.Users.Find(id);
            if (user.PowerUser == true)
            {
                return View(RedirectToAction("Index", "Admin"));
            }
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
            ViewBag.Active = "admin";
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