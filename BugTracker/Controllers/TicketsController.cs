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
using BugTracker.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;
using System.Security.Claims;

namespace BugTracker.Controllers
{
    [System.Web.Mvc.Authorize]
    [RequireHttps]
    public class TicketsController : ApplicationBaseController
    {
        TicketHelpers ticketHelper = new TicketHelpers();
        // GET: Tickets
        public ActionResult Index()
        {
            ViewBag.active = "tickets";
            UserProjectHelper helper = new UserProjectHelper();
            TicketsIndexViewModel vm = new TicketsIndexViewModel();
            var userId = User.Identity.GetUserId();
            var projects = helper.ListUserProjects(User.Identity.GetUserId());
            if (User.IsInRole("Admin"))
            {
                vm.Tickets = db.Tickets.ToList();
            }
            else if (User.IsInRole("ProjectManager"))
            {
                vm.Tickets = ticketHelper.ListPmTickets(User.Identity.GetUserId()).ToList();
            }
            else if (User.IsInRole("Developer"))
            {
                vm.Tickets = ticketHelper.ListDevTickets(User.Identity.GetUserId()).ToList();
            }
            else if (User.IsInRole("Submitter"))
            {
                vm.Tickets = ticketHelper.ListSubTickets(User.Identity.GetUserId()).ToList();
                if (db.Users.Find(userId).Projects.Count > 0)
                {
                    vm.HasProjects = true;
                }
            }
            return View(vm);
        }

        // GET: Tickets/Create
        [System.Web.Mvc.Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            ViewBag.active = "tickets";
            UserProjectHelper helper = new UserProjectHelper();
            NewTicketViewModel vm = new NewTicketViewModel();
            if (helper.ListUserProjects(User.Identity.GetUserId()).Count > 0)
            {
                vm.ProjectList = new SelectList(helper.ListUserProjects(User.Identity.GetUserId()), "Id", "Title");
                vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name");
                vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name");
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        // GET: Tickets/Create
        [System.Web.Mvc.Authorize(Roles = "Submitter")]
        public ActionResult CreateForProject(int? id)
        {
            ViewBag.active = "tickets";
            UserProjectHelper helper = new UserProjectHelper();
            NewTicketForProjectViewModel vm = new NewTicketForProjectViewModel();
            if (id != null)
            {
                var project = db.Projects.Find(id);
                if (project != null)
                {
                    if (helper.UserIsOnProject(User.Identity.GetUserId(), id.Value))
                    {
                        vm.DisplayProject = project.Title;
                        vm.StaticProjectId = project.Id;
                        vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name");
                        vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name");
                        return View(vm);
                    }
                }
            }
            return RedirectToAction("Index", "Projects");
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = "Submitter")]
        public ActionResult CreateForProject(NewTicketForProjectViewModel vm)
        {
            UserProjectHelper helper = new UserProjectHelper();
            if (ModelState.IsValid)
            {
                var project = db.Projects.Find(vm.StaticProjectId);
                if (helper.UserIsOnProject(User.Identity.GetUserId(), project.Id))
                {
                    var ticket = new Ticket();
                    ticket.TicketStatusId = 4;
                    ticket.OwnerUserId = User.Identity.GetUserId();
                    ticket.Created = DateTimeOffset.Now;
                    ticket.ProjectId = vm.StaticProjectId;
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

                    var count = new TicketCount();
                    count.ProjectId = project.Id;
                    count.OpenedCount = project.CurrentCreated + 1;
                    count.ResolvedCount = project.CurrentResolved;
                    count.Date = DateTimeOffset.Now;
                    project.TicketCounts.Add(count);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            if (!ModelState.IsValid)
            {
                vm.TypeList = new SelectList(db.TicketTypes, "Id", "Name", vm.TicketTypeId);
                vm.PriorityList = new SelectList(db.TicketPriorities, "Id", "Name", vm.TicketPriorityId);
                return View(vm);
            }
            return View(vm);

        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = "Submitter")]
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

                var project = db.Projects.Find(vm.ProjectId);
                var count = new TicketCount();
                count.ProjectId = vm.ProjectId;
                count.OpenedCount = project.CurrentCreated + 1;
                count.ResolvedCount = project.CurrentResolved;
                count.Date = DateTimeOffset.Now;
                project.TicketCounts.Add(count);

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

        public ActionResult UnclickAlert(int id)
        {
            var history = db.TicketHistories.Find(id);
            history.IsClicked = true;
            db.Entry(history).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = history.Ticket.Id });
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
        public async Task<ActionResult> Edit(EditTicketViewModel vm)
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
                    (User.IsInRole("Developer") && ticket.AssignToUserId == user.Id) ||
                    (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id))
                {
                    ticket.Updated = DateTimeOffset.Now;
                    if (ticket.Title != vm.Title)
                    {
                        if ((ticket.AssignToUserId != null && vm.AssignToUserId == null) ||
                            (ticket.AssignToUserId != null && vm.AssignToUserId == ticket.AssignToUserId))
                        {
                            var history = new TicketHistory { PropertyId = 29, ActionId = 4, OldValue = ticket.Title, NewValue = vm.Title, IsNotification = true, DeveloperId = ticket.AssignToUserId };
                            histories.Add(history);
                        }
                        else
                        {
                            var history = new TicketHistory { PropertyId = 29, ActionId = 4, OldValue = ticket.Title, NewValue = vm.Title, IsNotification = false };
                            histories.Add(history);
                        }
                    }
                    ticket.Title = vm.Title;
                    if (ticket.Description != vm.Description)
                    {
                        if ((ticket.AssignToUserId != null && vm.AssignToUserId == null) ||
                            (ticket.AssignToUserId != null && vm.AssignToUserId == ticket.AssignToUserId))
                        {
                            var history = new TicketHistory { PropertyId = 30, ActionId = 4, OldValue = ticket.Description, NewValue = vm.Description, IsNotification = true, DeveloperId = ticket.AssignToUserId };
                            histories.Add(history);
                        }
                        else
                        {
                            var history = new TicketHistory { PropertyId = 30, ActionId = 4, OldValue = ticket.Description, NewValue = vm.Description };
                            histories.Add(history);
                        }
                    }
                    ticket.Description = vm.Description;
                    if (ticket.TicketTypeId != vm.TicketTypeId)
                    {
                        if ((ticket.AssignToUserId != null && vm.AssignToUserId == null) ||
                            (ticket.AssignToUserId != null && vm.AssignToUserId == ticket.AssignToUserId))
                        {
                            var history = new TicketHistory { PropertyId = 31, ActionId = 4, OldValue = ticket.TicketType.Name, NewValue = db.TicketTypes.Find(vm.TicketTypeId).Name, IsNotification = true, DeveloperId = ticket.AssignToUserId };
                            histories.Add(history);
                        }
                        else
                        {
                            var history = new TicketHistory { PropertyId = 31, ActionId = 4, OldValue = ticket.TicketType.Name, NewValue = db.TicketTypes.Find(vm.TicketTypeId).Name };
                            histories.Add(history);
                        }
                    }
                    ticket.TicketTypeId = vm.TicketTypeId;
                    if (ticket.TicketPriorityId != vm.TicketPriorityId)
                    {
                        if ((ticket.AssignToUserId != null && vm.AssignToUserId == null) ||
                            (ticket.AssignToUserId != null && vm.AssignToUserId == ticket.AssignToUserId))
                        {
                            var history = new TicketHistory { PropertyId = 32, ActionId = 4, OldValue = ticket.TicketPriority.Name, NewValue = db.TicketPriorities.Find(vm.TicketPriorityId).Name, IsNotification = true, DeveloperId = ticket.AssignToUserId };
                            histories.Add(history);
                        }
                        else
                        {
                            var history = new TicketHistory { PropertyId = 32, ActionId = 4, OldValue = ticket.TicketPriority.Name, NewValue = db.TicketPriorities.Find(vm.TicketPriorityId).Name };
                            histories.Add(history);
                        }
                    }
                    ticket.TicketPriorityId = vm.TicketPriorityId;
                    if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
                    {
                        if (ticket.TicketStatusId != vm.TicketStatusId)
                        {
                            if (ticket.AssignToUserId != null && vm.AssignToUserId == ticket.AssignToUserId)
                            {
                                var history = new TicketHistory { PropertyId = 33, ActionId = 4, OldValue = ticket.TicketStatus.Name, NewValue = db.TicketStatuses.Find(vm.TicketStatusId).Name, IsNotification = true, DeveloperId = ticket.AssignToUserId };
                                histories.Add(history);
                            }
                            else
                            {
                                var history = new TicketHistory { PropertyId = 33, ActionId = 4, OldValue = ticket.TicketStatus.Name, NewValue = db.TicketStatuses.Find(vm.TicketStatusId).Name };
                                histories.Add(history);
                            }
                        }
                        ticket.TicketStatusId = vm.TicketStatusId;
                        if (ticket.AssignToUserId != vm.AssignToUserId && ticket.AssignToUserId != null)
                        {
                            var history = new TicketHistory { PropertyId = 34, ActionId = 4, OldValue = ticket.AssignToUser.FullName, NewValue = db.Users.Find(vm.AssignToUserId).FullName, IsNotification = true, DeveloperId = vm.AssignToUserId };
                            var oldUser = db.Users.Find(ticket.AssignToUserId);
                            foreach (var oldHistory in oldUser.Histories.Where(h => h.TicketId == ticket.Id).ToList())
                            {
                                oldUser.Histories.Remove(oldHistory);
                                db.SaveChanges();
                            }
                            histories.Add(history);
                            await SendAssignmentEmail(vm.AssignToUserId, ticket.Id);
                        }
                        if (ticket.AssignToUserId != vm.AssignToUserId && ticket.AssignToUserId == null)
                        {
                            var history = new TicketHistory { PropertyId = 34, ActionId = 3, NewValue = db.Users.Find(vm.AssignToUserId).FullName, IsNotification = true, DeveloperId = vm.AssignToUserId };
                            histories.Add(history);
                            await SendAssignmentEmail(vm.AssignToUserId, ticket.Id);
                        }
                        ticket.AssignToUserId = vm.AssignToUserId;
                    }
                    var hub = GlobalHost.ConnectionManager.GetHubContext<AlertsHub>();
                    foreach (var change in histories)
                    {
                        change.AuthorId = user.Id;
                        change.Created = DateTimeOffset.Now;
                        change.TicketId = ticket.Id;
                        change.Property = db.TicketHistoryProperties.Find(change.PropertyId);
                        change.Action = db.TicketHistoryActions.Find(change.ActionId);
                        if (change.IsNotification == true && change.DeveloperId != null)
                        {
                            db.Users.Find(change.DeveloperId).Histories.Add(change);
                            db.Entry(ticket).State = EntityState.Modified;
                            db.SaveChanges();
                            if (change.OldValue != null && change.NewValue != null)
                            {
                                Alert NewAlert = new Alert
                                {
                                    DisplayAlert = String.Format("{0} {1} {2} from {3} to {4}", change.Author.FirstName, change.Action.Name.ToLower(), change.Property.Name.ToLower(), change.OldValue, change.NewValue),
                                    IsClicked = change.IsClicked,
                                    DisplayTime = change.Created,
                                    LinkTicketId = change.Ticket.Id,
                                    LinkAlertId = change.Id
                                };
                                hub.Clients.All.Send(NewAlert.DisplayAlert, NewAlert.LinkAlertId);
                            }
                            if (change.OldValue == null && change.NewValue != null)
                            {
                                Alert NewAlert = new Alert
                                {
                                    DisplayAlert = String.Format("{0} {1} {2} {3}", change.Author.FirstName, change.Action.Name.ToLower(), change.Property.Name.ToLower(), change.NewValue),
                                    IsClicked = change.IsClicked,
                                    DisplayTime = change.Created,
                                    LinkTicketId = change.Ticket.Id,
                                    LinkAlertId = change.Id
                                };
                                hub.Clients.All.Send(NewAlert.DisplayAlert, NewAlert.LinkAlertId);
                            }
                        }
                        else
                        {
                            db.TicketHistories.Add(change);
                            db.Entry(ticket).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    if (histories.Count() > 0 && (vm.AssignToUserId != null && ticket.AssignToUserId != null))
                    {
                        await SendEditEmail(vm.AssignToUserId, ticket.Id);
                    }
                    if (histories.Count() > 0 && ticket.AssignToUserId != null && vm.AssignToUserId == null)
                    {
                        await SendEditEmail(ticket.AssignToUserId, ticket.Id);
                    }

                    return RedirectToAction("Details", new { id = vm.Id });
                }
                return RedirectToAction("Index");
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
            return RedirectToAction("Index", "Tickets");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public async Task SendAssignmentEmail(string userId, int ticketId)
        {
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketId }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, "You were assigned to a ticket", "you were assigned to a <a href=\"" + callbackUrl + "\">ticket</a>");
        }

        public async Task SendEditEmail(string userId, int ticketId)
        {
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketId }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userId, "Ticket Edited", "A <a href=\"" + callbackUrl + "\">ticket</a> you were assigned to has been edited.");
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

        //public void PushAlerts(TicketHistory alert)
        //{
        //    var author = db.Users.Find(alert.AuthorId).FirstName;
        //    var action = db.TicketHistoryActions.Find(alert.ActionId).Name;
        //    var property = db.TicketHistoryProperties.Find(alert.PropertyId).Name;
        //    var displayAlert = String.Concat(author, " ", action, " ", property, " from ", alert.OldValue, " to ", alert.NewValue, " at ", alert.Created.ToString());
        //    AlertHub.GetAlertFromServer(displayAlert);
        //}


    }
}
