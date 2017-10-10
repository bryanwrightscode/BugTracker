using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [Authorize]
    [RequireHttps]
    public class AttachmentsController : ApplicationBaseController
    {
        [HttpPost]
        public ActionResult Create(TicketAttachmentEditModel em)
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

                    var history = new TicketHistory();
                    history.TicketId = ticket.Id;
                    history.AuthorId = User.Identity.GetUserId();
                    history.Created = DateTimeOffset.Now;
                    history.PropertyId = 35;
                    history.ActionId = 6;
                    history.NewValue = attachment.FileName;
                    db.TicketHistories.Add(history);

                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = em.Id });
                }
            }
            return RedirectToAction("Details", "Tickets", new { id = em.Id });
        }
    }
}