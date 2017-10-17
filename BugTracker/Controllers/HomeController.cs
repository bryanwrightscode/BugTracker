using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BugTracker.Controllers
{
    public class HomeController : ApplicationBaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            UserProjectHelper helper = new UserProjectHelper();
            UserRoleHelper roleHelper = new UserRoleHelper();
            var user = db.Users.Find(User.Identity.GetUserId());
            DashboardViewModel dashItems = new DashboardViewModel();

            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                dashItems.User = user;
                dashItems.All = db.Projects.ToList();
                dashItems.Assigned = helper.ListUserProjects(user.Id);
                dashItems.AllTickets = db.Tickets.ToList();
                dashItems.MyTickets = db.Tickets.Where(t => t.Project.Users.Any(u => u.Id == user.Id)).ToList();
                dashItems.PmUnassignedTicketsCount = dashItems.MyTickets.Where(t => t.TicketStatus.Name == "Unassigned").Count().ToString();
                if (User.IsInRole("Admin"))
                {
                    dashItems.AdminUnassignedTicketsCount = db.Tickets.Where(t => t.TicketStatus.Name == "Unassigned").Count().ToString();
                }
            }
            if (User.IsInRole("Submitter"))
            {
                dashItems.SubmitterTicketsCount = db.Tickets.Where(t => t.OwnerUserId == user.Id).Count().ToString();
            }
            if (User.IsInRole("Developer"))
            {
                dashItems.DevTicketsCount = db.Tickets.Where(t => t.AssignToUserId == user.Id).Count().ToString();
            }
            dashItems.User = user;
            dashItems.Assigned = helper.ListUserProjects(user.Id);
            dashItems.UserRole = roleHelper.ListUserRoles(user.Id).ToArray();
            dashItems.ChartJsLables = new List<string>();
            dashItems.ChartJsValues = new List<int>();
            var projects = db.Projects.ToList();
            foreach (var project in projects)
            {
                dashItems.ChartJsLables.Add(project.Title);
                dashItems.ChartJsValues.Add(project.Tickets.Count());
            }
            dashItems.ChartJsLables = dashItems.ChartJsLables.ToArray();
            dashItems.ChartJsValues = dashItems.ChartJsValues.ToArray();

            return View(dashItems);
        }

        [AllowAnonymous]
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public ActionResult Landing(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}