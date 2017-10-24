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
            ViewBag.active = "dashboard";
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

            if (User.IsInRole("Admin"))
            {
                dashItems.ChartJsLables = new List<string>();
                dashItems.ChartJsValues = new List<int>();
                var projects = db.Projects.Where(p => p.Tickets.Count() > 0).ToList();
                foreach (var project in projects)
                {
                    dashItems.ChartJsLables.Add(project.Title);
                    dashItems.ChartJsValues.Add(project.Tickets.Count());
                }
                dashItems.ChartJsLables = dashItems.ChartJsLables.ToArray();
                dashItems.ChartJsValues = dashItems.ChartJsValues.ToArray();


                var ticketStatusChart = new Chart();
                ticketStatusChart.Labels = new List<string>();
                ticketStatusChart.Values = new List<int>();
                var statuses = db.TicketStatuses.ToList();
                foreach (var status in statuses)
                {
                    ticketStatusChart.Labels.Add(status.Name);
                    ticketStatusChart.Values.Add(db.Tickets.Where(t => t.TicketStatus.Name == status.Name).Count());
                }
                dashItems.TicketStatusChart = ticketStatusChart;
                dashItems.TicketStatusChart.Labels = ticketStatusChart.Labels.ToArray();
                dashItems.TicketStatusChart.Values = ticketStatusChart.Values.ToArray();

                var lineChart = new TicketStatusLineChart();
                lineChart.Dates = new List<string>();
                lineChart.Opened = new List<int>();
                lineChart.Resolved = new List<int>();
                var counts = db.TicketCounts;
                foreach (var count in counts)
                {
                    var timezoneId = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone);
                    var newTime = TimeZoneInfo.ConvertTime(count.Date, timezoneId).ToString("MM/dd/yy h:mm tt");
                    lineChart.Dates.Add(newTime);
                    lineChart.Opened.Add(count.TotalOpenedCount);
                    lineChart.Resolved.Add(count.TotalResolvedCount);
                }
                dashItems.TicketStatusLineChart = lineChart;
            }
            if (User.IsInRole("ProjectManager"))
            {
                var lineCharts = new List<TicketStatusLineChart>();
                var projects = user.Projects.ToList();
                var counts = db.TicketCounts.ToList();
                foreach (var project in projects)
                {
                    var chart = new TicketStatusLineChart();
                    chart.Dates = new List<string>();
                    chart.Opened = new List<int>();
                    chart.Resolved = new List<int>();
                    chart.ProjectId = project.Id;
                    chart.ProjectName = project.Title;
                    foreach (var count in counts.Where(c => c.ProjectId == project.Id))
                    {
                        var timezoneId = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone);
                        var newTime = TimeZoneInfo.ConvertTime(count.Date, timezoneId).ToString("MM/dd/yy h:mm tt");
                        chart.Dates.Add(newTime);
                        chart.Opened.Add(count.OpenedCount);
                        chart.Resolved.Add(count.ResolvedCount);
                    }
                    lineCharts.Add(chart);
                }

                var donutCharts = new List<Chart>();
                var statuses = db.TicketStatuses.ToList();
                foreach (var project in projects)
                {
                    var chart = new Chart();
                    chart.Labels = new List<string>();
                    chart.Values = new List<int>();
                    chart.ProjectName = project.Title;
                    chart.ProjectId = project.Id;
                    foreach (var status in statuses)
                    {
                        chart.Labels.Add(status.Name);
                        chart.Values.Add(project.Tickets.Where(t => t.TicketStatus.Name == status.Name).Count());
                    }
                    donutCharts.Add(chart);
                }
                dashItems.LineCharts = lineCharts;
                dashItems.DonutCharts = donutCharts;
            }

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