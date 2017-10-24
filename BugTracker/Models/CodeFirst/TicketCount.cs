using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.CodeFirst
{
    public class TicketCount
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTimeOffset Date { get; set; }
        public int OpenedCount { get; set; }
        public int ResolvedCount { get; set; }
        public int TotalOpenedCount { get; set; }
        public int TotalResolvedCount { get; set; }

        public virtual Project Project { get; set; }
    }
}