﻿using BugTracker.Models.CodeFirst;
using BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class DashboardViewModel : ApplicationBaseViewModel
    {
        public ApplicationUser User { get; set; }
        public ICollection<Project> Assigned { get; set; }
        public ICollection<Project> All { get; set; }
        public ICollection<Ticket> MyTickets { get; set; }
        public ICollection<Ticket> AllTickets { get; set; }
        public string SubmitterTicketsCount { get; set; }
        public string DevTicketsCount { get; set; }
        public string PmUnassignedTicketsCount { get; set; }
        public string AdminUnassignedTicketsCount { get; set; }
        public string[] UserRole { get; set; }
        public ICollection<string> ChartJsLables { get; set; }
        public ICollection<int> ChartJsValues { get; set; }
        public virtual Chart TicketStatusChart { get; set; }
    }

    public class Chart
    {
        public ICollection<string> Labels { get; set; }
        public ICollection<int> Values { get; set; }
    }

    public class TicketStatusChart
    {
        public ICollection<DateTimeOffset> Dates { get; set; }
        public ICollection<int> Opened { get; set; }
        public ICollection<int> Resolved { get; set; }
    }
}