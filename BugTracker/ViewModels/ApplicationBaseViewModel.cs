using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class ApplicationBaseViewModel
    {
        public ApplicationUser CurrentUser { get; set; }
        public ICollection<TicketHistory> IsNotificationHistories { get; set; }
        public string[] Role { get; set; }
    }
}