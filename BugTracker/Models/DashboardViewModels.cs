using BugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public ICollection<Project> Assigned { get; set; }
        public ICollection<Project> All { get; set; }
    }
}