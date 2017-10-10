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
        TicketHelpers ticketHelper = new TicketHelpers();

        // GET: Tickets
        public ActionResult Index()
        {
            ViewBag.active = "tickets";
            UserProjectHelper helper = new UserProjectHelper();
            var projects = helper.ListUserProjects(User.Identity.GetUserId());
            if (User.IsInRole("Admin"))
            {
                return View(db.Tickets.ToList());
            }
            if (User.IsInRole("ProjectManager"))
            {
                return View(ticketHelper.ListPmTickets(User.Identity.GetUserId()).ToList());
            }
            if (User.IsInRole("Developer"))
            {
                return View(ticketHelper.ListDevTickets(User.Identity.GetUserId()).ToList());
            }
            if (User.IsInRole("Submitter"))
            {
                return View(ticketHelper.ListSubTickets(User.Identity.GetUserId()).ToList());
            }
            return RedirectToAction("Index", "Home");
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

                var history = new TicketHistory();
                history.TicketId = ticket.Id;
                history.AuthorId = User.Identity.GetUserId();
                history.Created = DateTimeOffset.Now;
                history.PropertyId = 15;
                history.ActionId = 1;
                db.TicketHistories.Add(history);
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
                    vm.TicketHistories = ticket.Histories.OrderByDescending(c => c.Created).ToList();
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
            var ticket = db.Tickets.Find(id);
            vm.Id = id;
            vm.Title = ticket.Title;
            vm.TicketTypeId = ticket.TicketTypeId;
            vm.TicketPriorityId = ticket.TicketPriorityId;
            vm.Description = ticket.Description;
            if (ticket != null)
            {
                if (User.IsInRole("Admin"))
                {
                    vm.TicketStatusId = ticket.TicketStatusId;
                    vm.StatusList = new SelectList(db.TicketStatuses, "Id", "Name", vm.TicketStatusId);
                    vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name", vm.TicketTypeId);
                    vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name", vm.TicketPriorityId);
                    vm.AssignToUserId = ticket.AssignToUserId;
                    if (roleHelper.ListRoleUsers("Developer").Where(u => helper.UserIsOnProject(u.Id, ticket.ProjectId)).Count() > 0)
                    {
                        vm.AssignToUserList = new SelectList(roleHelper.ListRoleUsers("Developer").Where(u => helper.UserIsOnProject(u.Id, ticket.ProjectId)), "Id", "FullName", vm.AssignToUserId);
                    }
                    else
                    {
                        vm.AssignToUserList = null;
                    }
                    return View(vm);
                }
                if (User.IsInRole("ProjectManager") && helper.UserIsOnProject(user.Id, ticket.ProjectId))
                {
                    vm.StatusList = new SelectList(db.TicketStatuses, "Id", "Name", vm.TicketStatusId);
                    vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name", vm.TicketTypeId);
                    vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name", vm.TicketPriorityId);
                    return View(vm);
                }
                if (User.IsInRole("Developer") && ticket.AssignToUserId == user.Id)
                {
                    vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name", vm.TicketTypeId);
                    vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name", vm.TicketPriorityId);
                    return View(vm);
                }
                if (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id)
                {
                    vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name", vm.TicketTypeId);
                    vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name", vm.TicketPriorityId);
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
            Ticket ticket = db.Tickets.Find(vm.Id);
            UserProjectHelper helper = new UserProjectHelper();
            UserRoleHelper roleHelper = new UserRoleHelper();
            var user = db.Users.Find(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                HashSet<TicketHistory> histories = new HashSet<TicketHistory>();
                if (User.IsInRole("Admin") ||
                    (User.IsInRole("ProjectManager") && helper.UserIsOnProject(user.Id, ticket.ProjectId)) ||
                    (User.IsInRole("Developer") && vm.AssignToUserId == user.Id) ||
                    (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id))
                {
                    ticket.Updated = DateTimeOffset.Now;
                    if (ticket.Title != vm.Title)
                    {
                        var history = new TicketHistory { PropertyId = 29, ActionId = 4, OldValue = ticket.Title, NewValue = vm.Title };
                        histories.Add(history);
                    }
                    ticket.Title = vm.Title;
                    if (ticket.Description != vm.Description)
                    {
                        var history = new TicketHistory { PropertyId = 30, ActionId = 4, OldValue = ticket.Description, NewValue = vm.Description };
                        histories.Add(history);
                    }
                    ticket.Description = vm.Description;
                    if (ticket.TicketTypeId != vm.TicketTypeId)
                    {
                        var history = new TicketHistory { PropertyId = 31, ActionId = 4, OldValue = ticket.TicketType.Name, NewValue = db.TicketTypes.Find(vm.TicketTypeId).Name };
                        histories.Add(history);
                    }
                    ticket.TicketTypeId = vm.TicketTypeId;
                    if (ticket.TicketPriorityId != vm.TicketPriorityId)
                    {
                        var history = new TicketHistory { PropertyId = 32, ActionId = 4, OldValue = ticket.TicketPriority.Name, NewValue = db.TicketPriorities.Find(vm.TicketPriorityId).Name };
                        histories.Add(history);
                    }
                    ticket.TicketPriorityId = vm.TicketPriorityId;
                    if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
                    {
                        if (ticket.TicketStatusId != vm.TicketStatusId)
                        {
                            var history = new TicketHistory { PropertyId = 33, ActionId = 4, OldValue = ticket.TicketStatus.Name, NewValue = db.TicketStatuses.Find(vm.TicketStatusId).Name };
                            histories.Add(history);
                        }
                        ticket.TicketStatusId = vm.TicketStatusId;
                        if (ticket.AssignToUserId != vm.AssignToUserId && ticket.AssignToUserId != null)
                        {
                            var history = new TicketHistory { PropertyId = 34, ActionId = 4, OldValue = ticket.AssignToUser.FullName, NewValue = db.Users.Find(vm.AssignToUserId).FullName };
                            histories.Add(history);
                        }
                        else
                        {
                            var history = new TicketHistory { PropertyId = 34, ActionId = 3, NewValue = db.Users.Find(vm.AssignToUserId).FullName };
                            histories.Add(history);
                        }
                        ticket.AssignToUserId = vm.AssignToUserId;
                    }
                    foreach (var change in histories)
                    {
                        change.AuthorId = user.Id;
                        change.Created = DateTimeOffset.Now;
                        change.TicketId = ticket.Id;
                    }
                    db.TicketHistories.AddRange(histories);
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            if (!ModelState.IsValid)
            {
                vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name", vm.TicketTypeId);
                vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name", vm.TicketPriorityId);
                if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
                {
                    vm.StatusList = new SelectList(db.TicketStatuses, "Id", "Name", vm.TicketStatusId);
                    if (roleHelper.ListRoleUsers("Developer").Where(u => helper.UserIsOnProject(u.Id, ticket.ProjectId)).Count() > 0)
                    {
                        vm.AssignToUserList = new SelectList(roleHelper.ListRoleUsers("Developer").Where(u => helper.UserIsOnProject(u.Id, ticket.ProjectId)), "Id", "FullName", vm.AssignToUserId);
                    }
                    else
                    {
                        vm.AssignToUserList = null;
                    }
                }
                return View(vm);
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
