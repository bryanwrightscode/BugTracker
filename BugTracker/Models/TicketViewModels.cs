using BugTracker.Models.CodeFirst;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class NewTicketViewModel
    {
        public Ticket Ticket { get; set; }
        public int ProjectId { get; set; }
        public SelectList Projects { get; set; }
        public int TicketTypeId { get; set; }
        public SelectList TicketTypes { get; set; }
        public int TicketPriorityId { get; set; }
        public SelectList TicketPriorities { get; set; }
    }
}