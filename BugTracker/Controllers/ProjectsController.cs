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
using Microsoft.AspNet.Identity;
using BugTracker.Models.Helpers;

namespace BugTracker.Controllers
{
    public class ProjectsController : ApplicationBaseController
    {
        //// GET: Projects
        //[Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        //public ActionResult Index()
        //{
        //    UserProjectHelper helper = new UserProjectHelper();
        //    List<ProjectViewModel> projects = new List<ProjectViewModel>();

        //    foreach (var project in db.Projects.ToList())
        //    {
        //        ProjectViewModel eachProject = new ProjectViewModel();
        //        var user = User.Identity.GetUserId();
        //        eachProject.User = helper.ProjectAuthor(project.Id);
        //        eachProject.Assigned = helper.UserProject(user, project.Id);
        //        eachProject.All = project;
        //        projects.Add(eachProject);

        //    }
        //    return View(projects);
        //}

        //Get: Projects
        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        public ActionResult Index()
        {
            ViewBag.active = "projects";
            UserProjectHelper helper = new UserProjectHelper();
            AllAndAssignedProjectsViewModel vm = new AllAndAssignedProjectsViewModel();
            var user = User.Identity.GetUserId();
            vm.Assigned = helper.ListUserProjects(user);
            vm.All = db.Projects.ToList();
            return View(vm);
        }

        public ActionResult Details(int id)
        {
            ViewBag.active = "projects";
            ProjectDetailViewModel vm = new ProjectDetailViewModel();
            UserProjectHelper projectHelper = new UserProjectHelper();
            UserRoleHelper roleHelper = new UserRoleHelper();
            TicketHelpers ticketHelper = new TicketHelpers();
            var user = db.Users.Find(User.Identity.GetUserId());
            var project = db.Projects.Find(id);
            if (project != null)
            {
                if ((User.IsInRole("Admin")) || 
                    ((User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter")) &&
                    projectHelper.UserIsOnProject(user.Id, project.Id)))
                {
                    vm.Project = project;
                    if (project.Users.Where(u => roleHelper.UserIsInRole(u.Id, "ProjectManager")).Count() > 0)
                    {
                        vm.ProjectManager = project.Users.Where(u => roleHelper.UserIsInRole(u.Id, "ProjectManager")).First().FullName;
                    }
                    if (project.Tickets.Count() > 0)
                    {
                        vm.Tickets = project.Tickets.OrderByDescending(t => t.Created).ToList();
                        if (User.IsInRole("Developer"))
                        {
                            vm.TicketsOn = ticketHelper.ListDevTickets(user.Id).OrderByDescending(t => t.Created).ToList();
                        }
                        if (User.IsInRole("Submitter"))
                        {
                            vm.TicketsOn = ticketHelper.ListSubTickets(user.Id).OrderByDescending(t => t.Created).ToList();
                        }
                    }
                    if (project.Users.Count() > 0)
                    {
                        var users = new List<string>();
                        foreach (var name in project.Users.OrderBy(u => u.LastName))
                        {
                            users.Add(name.FullName);
                        }
                        vm.Users = users;
                    }
                    return View(vm);
                }
            }
            return (RedirectToAction("Index"));
        }

        //Get
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Assign(int id)
        {
            ViewBag.active = "projects";
            var project = db.Projects.Find(id);
            UserProjectHelper projectHelper = new UserProjectHelper();
            if (!projectHelper.UserIsOnProject(User.Identity.GetUserId(), project.Id) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                ProjectAssignViewModel projectuserVM = new ProjectAssignViewModel();
                projectuserVM.AssignProject = project;
                projectuserVM.SelectedUsers = project.Users.Select(u => u.Id).ToArray();
                if (db.Users.Find(User.Identity.GetUserId()).PowerUser == true)
                {
                    projectuserVM.Users = new MultiSelectList(db.Users.ToList(), "Id", "FirstName", projectuserVM.SelectedUsers);
                }
                else
                {
                    projectuserVM.Users = new MultiSelectList(db.Users.Where(u => u.PowerUser == false).ToList(), "Id", "FirstName", projectuserVM.SelectedUsers);
                }
                return View(projectuserVM);
            }
        }

        //Post
        [HttpPost]
        [Authorize(Roles = "Admin, ProjectManager")]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(ProjectAssignViewModel model)
        {
            UserProjectHelper helper = new UserProjectHelper();
            var project = db.Projects.Find(model.AssignProject.Id);
            if (!helper.UserIsOnProject(User.Identity.GetUserId(), project.Id) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                var time = DateTimeOffset.Now;
                foreach (var userId in db.Users.Select(r => r.Id).ToList())
                {
                    if (!model.SelectedUsers.Any(u => u == userId))
                    {
                        var tickets = project.Tickets.Where(t => t.AssignToUserId == userId);
                        foreach (var ticket in tickets)
                        {
                            var history = new TicketHistory
                            {
                                PropertyId = 34,
                                ActionId = 9,
                                AuthorId = User.Identity.GetUserId(),
                                Created = DateTimeOffset.Now,
                                TicketId = ticket.Id,
                                OldValue = ticket.AssignToUser.FullName,
                                NewValue = null
                            };
                            db.TicketHistories.Add(history);
                            db.SaveChanges();
                            ticket.AssignToUserId = null;
                            ticket.Updated = DateTimeOffset.Now;
                            if (ticket.TicketStatusId == 5)
                            {
                                ticket.TicketStatusId = 4;
                            }
                        }
                    }
                    helper.RemoveUserFromProject(userId, project.Id);
                }
                foreach (var userId in model.SelectedUsers)
                {
                    helper.AddUserToProject(userId, project.Id);
                }
                project.Updated = time;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.active = "projects";
            CreateProjectViewModel vm = new CreateProjectViewModel();
            return View(vm);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateProjectViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var project = new Project();
                project.AuthorId = User.Identity.GetUserId();
                project.Created = DateTimeOffset.Now;
                project.Title = vm.Title;
                project.Description = vm.Description;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vm);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.active = "projects";
            EditProjectViewModel vm = new EditProjectViewModel();
            UserProjectHelper helper = new UserProjectHelper();
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                vm.Project = db.Projects.Find(id);
                if (vm.Project == null)
                {
                    return RedirectToAction("Index");
                }
                else if (!helper.UserIsOnProject(User.Identity.GetUserId(), vm.Project.Id) && !User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index");
                }
                return View(vm);
            }
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Description,AuthorId")] Project project)
        {
            UserProjectHelper helper = new UserProjectHelper();
            if (!helper.UserIsOnProject(User.Identity.GetUserId(), project.Id) && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var time = DateTimeOffset.Now;
                    project.Updated = time;
                    db.Entry(project).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(project);
            }
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
