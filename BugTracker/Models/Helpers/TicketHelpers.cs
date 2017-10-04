using BugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class TicketHelpers
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        ///<summary>Finds if ticket assigned project</summary>
        public bool TicketIsInProject(int ticketId, int projectId)
        {
            var ticket = db.Tickets.Find(ticketId);
            var project = db.Projects.Find(projectId);
            return project.Tickets.Any(t => t.Id == ticketId);
        }

        ///<summary>Adds ticket to project</summary>
        public void AddTicketToProject(int ticketId, int projectId)
        {
            var ticket = db.Tickets.Find(ticketId);
            var project = db.Projects.Find(projectId);
            project.Tickets.Add(ticket);
            db.SaveChanges();
        }

        ///<summary>Removes ticket from project</summary>
        public void RemoveTicketFromProject(int ticketId, int projectId)
        {
            var ticket = db.Tickets.Find(ticketId);
            var project = db.Projects.Find(projectId);
            project.Tickets.Remove(ticket);
            db.SaveChanges();
        }

        ///<summary>Lists project tickets</summary>
        public ICollection<Ticket> ListProjectTickets(int projectId)
        {
            var project = db.Projects.Find(projectId);
            return project.Tickets.ToList();
        }

        ///<summary>Lists non project tickets</summary>
        public ICollection<Ticket> ListNotProjectTickets(int projectId)
        {
            var project = db.Projects.Find(projectId);
            return db.Tickets.Where(t => t.ProjectId != projectId).ToList();
        }

        public ICollection<TicketType> ListTicketTypes()
        {
            return db.TicketTypes.ToList();
        }

        public ICollection<TicketPriority> ListTicketPriorities()
        {
            return db.TicketPriorities.ToList();
        }
    }
}