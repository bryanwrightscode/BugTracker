using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    [RequireHttps]
    public class AttachmentsController : ApplicationBaseController
    {
        [HttpPost]
        public async Task<ActionResult> Create(TicketAttachmentEditModel em)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.Find(em.Id);
                if (em.File != null && em.File.ContentLength > 0)
                {
                    var ext = Path.GetExtension(em.File.FileName).ToLower();
                    if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf" &&
                        ext != ".txt" && ext != ".docx" && ext != ".xlsx" && ext != ".csv" && ext != ".pptx")
                    {
                        ModelState.AddModelError("File", "Invalid file format");
                    }
                }
                if (em.File != null)
                {
                    var attachment = new TicketAttachment();
                    var filePath = "/Attachments/";
                    var absPath = Server.MapPath("~" + filePath);
                    attachment.FileName = em.File.FileName;
                    attachment.FileUrl = filePath + em.File.FileName;
                    em.File.SaveAs(Path.Combine(absPath, em.File.FileName));
                    attachment.TicketId = ticket.Id;
                    attachment.AuthorId = User.Identity.GetUserId();
                    attachment.Created = DateTimeOffset.Now;
                    attachment.Description = em.Description;
                    db.TicketAttachments.Add(attachment);
                    db.Entry(ticket).State = EntityState.Modified;

                    var history = new TicketHistory();
                    history.TicketId = ticket.Id;
                    history.AuthorId = User.Identity.GetUserId();
                    history.Created = DateTimeOffset.Now;
                    history.PropertyId = 35;
                    history.ActionId = 6;
                    history.NewValue = attachment.FileName;
                    if (ticket.AssignToUserId != null)
                    {
                        history.IsNotification = true;
                        history.DeveloperId = ticket.AssignToUserId;
                        db.Users.Find(history.DeveloperId).Histories.Add(history);
                        await SendEditEmail(ticket.AssignToUserId, ticket.Id);
                    }
                    else
                    {
                        db.TicketHistories.Add(history);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = em.Id });
                }
            }
            return RedirectToAction("Details", "Tickets", new { id = em.Id });
        }
        public async Task SendEditEmail(string userId, int ticketId)
        {
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketId }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, "Ticket Attachment", "A <a href=\"" + callbackUrl + "\">ticket</a> you were assigned has a new attachment.");
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