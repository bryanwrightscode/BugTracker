using BugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class AdminProjectsViewModel
    {
        public ApplicationUser User { get; set; }
        public Project Project { get; set; }
    }
}