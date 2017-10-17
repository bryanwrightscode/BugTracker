using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class CommentsController : ApplicationBaseController
    {
        [HttpPost]
        public async Task<ActionResult> Create(TicketCommentEditModel em)
        {
            em.Ticket = db.Tickets.Find(em.Ticket.Id);
            em.TicketComment.Created = DateTimeOffset.Now;
            em.TicketComment.AuthorId = User.Identity.GetUserId();
            em.TicketComment.TicketId = em.Ticket.Id;
            db.TicketComments.Add(em.TicketComment);

            var history = new TicketHistory();
            history.TicketId = em.Ticket.Id;
            history.AuthorId = User.Identity.GetUserId();
            history.Created = DateTimeOffset.Now;
            history.PropertyId = 36;
            history.ActionId = 2;
            history.NewValue = em.TicketComment.Body;
            if (em.Ticket.AssignToUserId != null)
            {
                history.IsNotification = true;
                history.DeveloperId = em.Ticket.AssignToUserId;
                db.Users.Find(history.DeveloperId).Histories.Add(history);
                await SendEditEmail(em.Ticket.AssignToUserId, em.Ticket.Id);
            }
            else
            {
                db.TicketHistories.Add(history);
            }
            db.SaveChanges();
            return (RedirectToAction("Details", "Tickets", new { id = em.Ticket.Id } ));
        }

        public async Task SendEditEmail(string userId, int ticketId)
        {
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketId }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, "Ticket Comment", "A <a href=\"" + callbackUrl + "\">ticket</a> you were assigned has a new comment.");
        }

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    }
}