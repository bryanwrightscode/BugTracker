using BugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectViewModel
    {
        public ApplicationUser User { get; set; }
        public Project Assigned { get; set; }
        public Project All { get; set; }
    }

    public class AllAndAssignedProjectsViewModel
    {
        public ICollection<Project> Assigned { get; set; }
        public ICollection<Project> All { get; set; }
    }

    public class ProjectAssignViewModel
    {
        public Project AssignProject { get; set; }
        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
    }
}