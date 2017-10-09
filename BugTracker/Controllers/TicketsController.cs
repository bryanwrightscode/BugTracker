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
        // GET: Tickets
        public ActionResult Index()
        {
            ViewBag.active = "tickets";
            UserProjectHelper helper = new UserProjectHelper();
            TicketHelpers ticketHelper = new TicketHelpers();
            var projects = helper.ListUserProjects(User.Identity.GetUserId());
            if (User.IsInRole("Admin"))
            {
                return View(db.Tickets.ToList());
            }
            if (User.IsInRole("ProjectManager"))
            {
                return View(ticketHelper.ListPmTickets(User.Identity.GetUserId()));
            }
            if (User.IsInRole("Developer"))
            {
                return View(ticketHelper.ListDevTickets(User.Identity.GetUserId()));
            }
            if (User.IsInRole("Submitter"))
            {
                return View(ticketHelper.ListSubTickets(User.Identity.GetUserId()));
            }
            return View(db.Tickets.ToList());
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            ViewBag.active = "tickets";
            UserProjectHelper helper = new UserProjectHelper();
            NewTicketViewModel vm = new NewTicketViewModel();
            vm.ProjectList = new SelectList(helper.ListUserProjects(User.Identity.GetUserId()), "Id", "Title");
            vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name");
            vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name");
            return View(vm);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(NewTicketViewModel vm)
        {
            UserProjectHelper helper = new UserProjectHelper();
            if (ModelState.IsValid)
            {
                var ticket = new Ticket();
                ticket.TicketStatusId = 4;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.Now;
                ticket.ProjectId = vm.ProjectId;
                ticket.TicketTypeId = vm.TicketTypeId;
                ticket.TicketPriorityId = vm.TicketPriorityId;
                ticket.Title = vm.Title;
                ticket.Description = vm.Description;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                vm.ProjectList = new SelectList(helper.ListUserProjects(User.Identity.GetUserId()), "Id", "Title", vm.ProjectId);
                vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name", vm.TicketTypeId);
                vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name", vm.TicketPriorityId);
                return View(vm);
            }
            return View(vm);

        }

        public ActionResult Details(int id)
        {
            ViewBag.active = "tickets";
            TicketDetailViewModel vm = new TicketDetailViewModel();
            UserProjectHelper projectHelper = new UserProjectHelper();
            UserRoleHelper roleHelper = new UserRoleHelper();
            var user = db.Users.Find(User.Identity.GetUserId());
            var ticket = db.Tickets.Find(id);
            if (ticket != null)
            {
                if (User.IsInRole("Admin") ||
                    (User.IsInRole("ProjectManager") && projectHelper.UserIsOnProject(user.Id, ticket.ProjectId)) ||
                    (User.IsInRole("Developer") && ticket.AssignToUserId == user.Id) ||
                    (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id))
                {
                    vm.Ticket = ticket;
                    vm.TicketComments = ticket.Comments.OrderBy(c => c.Created).ToList();
                    vm.TicketAttachments = ticket.Attachments.OrderByDescending(a => a.Created).ToList();
                    vm.User = user;
                    return View(vm);
                }
            }
            return (RedirectToAction("Index"));
        }

        public ActionResult Edit(int id)
        {
            ViewBag.active = "tickets";
            EditTicketViewModel vm = new EditTicketViewModel();
            UserProjectHelper helper = new UserProjectHelper();
            UserRoleHelper roleHelper = new UserRoleHelper();
            var user = db.Users.Find(User.Identity.GetUserId());
            vm.Ticket = db.Tickets.Find(id);
            if (vm.Ticket != null)
            {
                if (User.IsInRole("Admin"))
                {
                    vm.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", vm.Ticket.TicketStatusId);
                    vm.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", vm.Ticket.TicketTypeId);
                    vm.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", vm.Ticket.TicketPriorityId);
                    if (roleHelper.ListRoleUsers("Developer").Any(u => helper.UserIsOnProject(u.Id, vm.Ticket.ProjectId)))
                    {
                        vm.AssignToUserId = new SelectList(roleHelper.ListRoleUsers("Developer").Where(u => helper.UserIsOnProject(u.Id, vm.Ticket.ProjectId)), "Id", "FullName", vm.Ticket.AssignToUserId);
                    }
                    return View(vm);
                }
                if (User.IsInRole("ProjectManager") && helper.UserIsOnProject(user.Id, vm.Ticket.ProjectId))
                {
                    vm.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", vm.Ticket.TicketStatusId);
                    vm.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", vm.Ticket.TicketTypeId);
                    vm.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", vm.Ticket.TicketPriorityId);
                    vm.AssignToUserId = new SelectList(roleHelper.ListRoleUsers("Developer").Where(u => helper.UserIsOnProject(u.Id, vm.Ticket.ProjectId)), "Id", "FullName", vm.Ticket.AssignToUserId);
                    return View(vm);
                }
                if (User.IsInRole("Developer") && vm.Ticket.AssignToUserId == user.Id)
                {
                    vm.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", vm.Ticket.TicketTypeId);
                    vm.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", vm.Ticket.TicketPriorityId);
                    return View(vm);
                }
                if (User.IsInRole("Submitter") && vm.Ticket.OwnerUserId == user.Id)
                {
                    vm.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", vm.Ticket.TicketTypeId);
                    vm.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", vm.Ticket.TicketPriorityId);
                    return View(vm);
                }
                return (RedirectToAction("Index"));
            }

            return (RedirectToAction("Index"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditTicketViewModel vm)
        {
            Ticket ticket = db.Tickets.Find(vm.Ticket.Id);
            UserProjectHelper helper = new UserProjectHelper();
            var user = db.Users.Find(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    ticket.Updated = DateTimeOffset.Now;
                    ticket.Title = vm.Ticket.Title;
                    ticket.Description = vm.Ticket.Description;
                    ticket.TicketTypeId = vm.Ticket.TicketTypeId;
                    ticket.TicketPriorityId = vm.Ticket.TicketPriorityId;
                    ticket.TicketStatusId = vm.Ticket.TicketStatusId;
                    ticket.AssignToUserId = vm.Ticket.AssignToUserId;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                if (User.IsInRole("ProjectManager") && helper.UserIsOnProject(user.Id, vm.Ticket.ProjectId))
                {
                    ticket.Updated = DateTimeOffset.Now;
                    ticket.Title = vm.Ticket.Title;
                    ticket.Description = vm.Ticket.Description;
                    ticket.TicketTypeId = vm.Ticket.TicketTypeId;
                    ticket.TicketPriorityId = vm.Ticket.TicketPriorityId;
                    ticket.TicketStatusId = vm.Ticket.TicketStatusId;
                    ticket.AssignToUserId = vm.Ticket.AssignToUserId;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                if (User.IsInRole("Developer") && vm.Ticket.AssignToUserId == user.Id)
                {
                    ticket.Updated = DateTimeOffset.Now;
                    ticket.Title = vm.Ticket.Title;
                    ticket.Description = vm.Ticket.Description;
                    ticket.TicketTypeId = vm.Ticket.TicketTypeId;
                    ticket.TicketPriorityId = vm.Ticket.TicketPriorityId;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                if (User.IsInRole("Submitter") && vm.Ticket.OwnerUserId == user.Id)
                {
                    ticket.Updated = DateTimeOffset.Now;
                    ticket.Title = vm.Ticket.Title;
                    ticket.Description = vm.Ticket.Description;
                    ticket.TicketTypeId = vm.Ticket.TicketTypeId;
                    ticket.TicketPriorityId = vm.Ticket.TicketPriorityId;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(vm);
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
