using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class CommentsController : ApplicationBaseController
    {
        [HttpPost]
        public ActionResult Create(TicketCommentEditModel em)
        {
            em.Ticket = db.Tickets.Find(em.Ticket.Id);
            em.TicketComment.Created = DateTimeOffset.Now;
            em.TicketComment.AuthorId = User.Identity.GetUserId();
            em.TicketComment.TicketId = em.Ticket.Id;
            db.TicketComments.Add(em.TicketComment);
            db.SaveChanges();
            return (RedirectToAction("Details", "Tickets", new { id = em.Ticket.Id } ));
        }
    }
}