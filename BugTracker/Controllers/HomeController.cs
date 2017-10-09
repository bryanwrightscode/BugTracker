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
    public class HomeController : ApplicationBaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            UserProjectHelper helper = new UserProjectHelper();

            var user = db.Users.Find(User.Identity.GetUserId());
            DashboardViewModel dashItems = new DashboardViewModel();

            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                dashItems.User = user;
                dashItems.All = db.Projects.ToList();
                dashItems.Assigned = helper.ListUserProjects(user.Id);
                return View(dashItems);
            }
            dashItems.User = user;
            dashItems.Assigned = helper.ListUserProjects(user.Id);
            return View(dashItems);
        }
    }
}