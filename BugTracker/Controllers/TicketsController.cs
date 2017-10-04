using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using System.Runtime.Caching;
using AutoMapper;
using System.Collections;

namespace BugTracker.Controllers
{
    [Authorize]
    [RequireHttps]
    public class TicketsController : ApplicationBaseController
    {
        TicketHelpers helper = new TicketHelpers();

        // GET: Tickets
        public ActionResult Index()
        {
            ViewBag.active = "tickets";
            //var tickets = db.Tickets.Include(t => t.AssignToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketStatus).Include(t => t.TicketType);
            //return View(tickets.ToList());
            return View(db.Tickets.ToList());
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.active = "tickets";
            NewTicketViewModel vm = new NewTicketViewModel();
            UserProjectHelper helper = new UserProjectHelper();
            var projects = helper.ListUserProjects(User.Identity.GetUserId());

            vm.Projects = new SelectList(projects, "Id", "Title", vm.ProjectId);
            vm.TicketTypes = new SelectList(db.TicketTypes, "Id", "Name", vm.TicketTypeId);
            vm.TicketPriorities = new SelectList(db.TicketPriorities, "Id", "Name", vm.TicketPriorityId);

            return View(vm);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewTicketViewModel vm)
        {
            UserProjectHelper helper = new UserProjectHelper();

            if (ModelState.IsValid)
            {
                vm.Ticket.TicketStatusId = 4;
                vm.Ticket.OwnerUserId = User.Identity.GetUserId();
                vm.Ticket.Created = DateTimeOffset.Now;
                vm.Ticket.ProjectId = vm.ProjectId;
                vm.Ticket.TicketTypeId = vm.TicketTypeId;
                vm.Ticket.TicketPriorityId = vm.TicketPriorityId;

                db.Tickets.Add(vm.Ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vm.Ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.active = "tickets";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
