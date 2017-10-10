using BugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
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

        public ICollection<Ticket> ListPmTickets(string userId)
        {
            UserProjectHelper helper = new UserProjectHelper();
            var projects = helper.ListUserProjects(userId);
            var tickets = projects.SelectMany(p => p.Tickets).ToList();
            return tickets;
        }

        public ICollection<Ticket> ListDevTickets(string userId)
        {
            return db.Tickets.Where(t => t.AssignToUserId == userId).ToList();
        }

        public ICollection<Ticket> ListSubTickets(string userId)
        {
            return db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
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

        ///<summary>Add ticket history item to current ticket based on property</summary>
        public void AddTicketHistory(int ticketId, int propertyId, string userId)
        {
            var ticket = db.Tickets.Find(ticketId);
            TicketHistory history = new TicketHistory();
            history.Ticket.Id = ticketId;
            history.Property.Id = propertyId;
            history.AuthorId = userId;
            ticket.Histories.Add(history);
            db.SaveChanges();
        }

        ///<summary>Add ticket history item to current ticket</summary>
        //public void AddTicketHistory(int ticketId, int historyId)
        //{
        //    var ticket = db.Tickets.Find(ticketId);
        //    var history = db.TicketHistories.Find(historyId);
        //    ticket.Histories.Add(history);
        //    db.SaveChanges();
        //}
    }
}