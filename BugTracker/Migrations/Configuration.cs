namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using BugTracker.Models.CodeFirst;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            if (!context.TicketPriorities.Any(p => p.Name == "Urgent"))
            {
                var priority = new TicketPriority();
                priority.Name = "Urgent";
                context.TicketPriorities.Add(priority);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "Low"))
            {
                var priority = new TicketPriority();
                priority.Name = "Low";
                context.TicketPriorities.Add(priority);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "Medium"))
            {
                var priority = new TicketPriority();
                priority.Name = "Medium";
                context.TicketPriorities.Add(priority);
            }
            if (!context.TicketPriorities.Any(p => p.Name == "High"))
            {
                var priority = new TicketPriority();
                priority.Name = "High";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "Unassigned"))
            {
                var status = new TicketStatus();
                status.Name = "Unassigned";
                context.TicketStatuses.Add(status);
            }
            if (!context.TicketStatuses.Any(p => p.Name == "Assigned"))
            {
                var status = new TicketStatus();
                status.Name = "Assigned";
                context.TicketStatuses.Add(status);
            }
            //if (!context.TicketStatuses.Any(p => p.Name == "In Progress"))
            //{
            //    var status = new TicketStatus();
            //    status.Name = "In Progress";
            //    context.TicketStatuses.Add(status);
            //}
            if (!context.TicketStatuses.Any(p => p.Name == "Resolved"))
            {
                var status = new TicketStatus();
                status.Name = "Resolved";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketTypes.Any(p => p.Name == "Hardware"))
            {
                var type = new TicketType();
                type.Name = "Hardware";
                context.TicketTypes.Add(type);
            }
            if (!context.TicketTypes.Any(p => p.Name == "Software"))
            {
                var type = new TicketType();
                type.Name = "Software";
                context.TicketTypes.Add(type);
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "ProductManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "bcwright03@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "bcwright03@gmail.com",
                    Email = "bcwright03@gmail.com",
                    FirstName = "Bryan",
                    LastName = "Wright"
                }, "Beagles4!");
            }
            var adminId = userManager.FindByEmail("bcwright03@gmail.com").Id;
            userManager.AddToRole(adminId, "Admin");

            if (!context.Users.Any(u => u.Email == "mjaang@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mjaang@coderfoundry.com",
                    Email = "mjaang@coderfoundry.com",
                    FirstName = "Mark",
                    LastName = "Jaang"
                }, "MarkAdminPassword");
            }
            var adminIdb = userManager.FindByEmail("mjaang@coderfoundry.com").Id;
            userManager.AddToRole(adminIdb, "Admin");

            if (!context.Users.Any(u => u.Email == "rchapman@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rchapman@coderfoundry.com",
                    Email = "rchapman@coderfoundry.com",
                    FirstName = "Ryan",
                    LastName = "Chapman"
                }, "RyanAdminPassword");
            }
            var adminIdc = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(adminIdc, "Admin");

            if (!context.Users.Any(u => u.Email == "admin@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "admin@coderfoundry.com",
                    Email = "admin@coderfoundry.com",
                    FirstName = "Admin",
                    LastName = "Demo"
                }, "DemoAdminPassword");
            }
            var adminIdDemo = userManager.FindByEmail("admin@coderfoundry.com").Id;
            userManager.AddToRole(adminIdDemo, "Admin");

            if (!context.Users.Any(u => u.Email == "projectmanager@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "projectmanager@coderfoundry.com",
                    Email = "projectmanager@coderfoundry.com",
                    FirstName = "Project Manager",
                    LastName = "Demo"
                }, "DemoProjectManagerPassword");
            }
            var projectManagerIdDemo = userManager.FindByEmail("projectmanager@coderfoundry.com").Id;
            userManager.AddToRole(projectManagerIdDemo, "ProjectManager");

            if (!context.Users.Any(u => u.Email == "developer@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "developer@coderfoundry.com",
                    Email = "developer@coderfoundry.com",
                    FirstName = "Developer",
                    LastName = "Demo"
                }, "DemoDeveloperPassword");
            }
            var developerIdDemo = userManager.FindByEmail("developer@coderfoundry.com").Id;
            userManager.AddToRole(developerIdDemo, "Developer");

            if (!context.Users.Any(u => u.Email == "submitter@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "submitter@coderfoundry.com",
                    Email = "submitter@coderfoundry.com",
                    FirstName = "Submitter",
                    LastName = "Demo"
                }, "DemoSubmitterPassword");
            }
            var submitterIdDemo = userManager.FindByEmail("submitter@coderfoundry.com").Id;
            userManager.AddToRole(submitterIdDemo, "Submitter");

            if (!context.Users.Any(u => u.Email == "shanda@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "shanda@coderfoundry.com",
                    Email = "shanda@coderfoundry.com",
                    FirstName = "Shanda",
                    LastName = "Submitter"
                }, "ShandaSubmitterPassword");
            }
            var submitterIdShanda = userManager.FindByEmail("Shanda@coderfoundry.com").Id;
            userManager.AddToRole(submitterIdShanda, "Submitter");
        }
    }
}
