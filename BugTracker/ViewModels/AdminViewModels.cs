using BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class AdminViewModels : ApplicationBaseViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }

    public class AdminIndexViewModel : ApplicationBaseViewModel
    {
        public ICollection<AdminUserRoles> Users { get; set; }
    }

    public class AdminUserRoles
    {
        public ApplicationUser User { get; set; }
        public string[] UserRoles { get; set; }
    }
}