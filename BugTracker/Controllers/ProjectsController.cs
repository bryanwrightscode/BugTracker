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
            AllAndAssignedProjectsViewModel projects = new AllAndAssignedProjectsViewModel();
            var user = User.Identity.GetUserId();
            projects.Assigned = helper.ListUserProjects(user);
            projects.All = db.Projects.ToList();
            return View(projects);
        }

        //Get
        [Authorize (Roles = "Admin, ProjectManager")]
        public ActionResult Assign(int id)
        {
            ViewBag.active = "projects";

            var project = db.Projects.Find(id);
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

        //Post
        [HttpPost]
        [Authorize (Roles = "Admin, ProjectManager")]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(ProjectAssignViewModel model)
        {
            UserProjectHelper helper = new UserProjectHelper();
            var project = db.Projects.Find(model.AssignProject.Id);
            var time = DateTimeOffset.Now;
            foreach (var userId in db.Users.Select(r => r.Id).ToList())
            {
                helper.RemoveUserFromProject(userId, project.Id);
            }
            foreach (var userId in model.SelectedUsers)
            {
                helper.AddUserToProject(userId, project.Id);
            }
            project.Updated = time;
            //db.Entry(project).State = EntityState.Modified;
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Create
        public ActionResult Create()
        {
            ViewBag.active = "projects";

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Description,AuthorId")] Project project)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var time = DateTimeOffset.Now;
                project.AuthorId = user.Id;
                project.Created = time;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.active = "projects";

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Description,AuthorId")] Project project)
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
