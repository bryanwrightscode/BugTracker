﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.CodeFirst
{
    public class TicketHistory
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public string AuthorId { get; set; }
        public string DeveloperId { get; set; }
        public int PropertyId { get; set; }
        public int ActionId { get; set; }

        public bool IsNotification { get; set; }
        public bool IsClicked { get; set; }
        public bool IsAlertArchived { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTimeOffset Created { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual ApplicationUser Developer { get; set; }
        public virtual TicketHistoryProperty Property { get; set; }
        public virtual TicketHistoryAction Action { get; set; }
    }
}