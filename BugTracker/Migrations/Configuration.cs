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
            //Ticket History Actions
            if (!context.TicketHistoryActions.Any(p => p.Name == "Created"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Create";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Added"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Add";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Assigned"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Assign";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Changed"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Change";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Edited"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Edit";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Uploaded"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Upload";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Updated"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Update";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Deleted"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Delete";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Removed"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Removed";
                context.TicketHistoryActions.Add(action);
            }
            if (!context.TicketHistoryActions.Any(p => p.Name == "Unassigned"))
            {
                var action = new TicketHistoryAction();
                action.Name = "Unassigned";
                context.TicketHistoryActions.Add(action);
            }

            //Ticket History Properties
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Ticket"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Ticket";
                context.TicketHistoryProperties.Add(property);
            }
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Title"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Title";
                context.TicketHistoryProperties.Add(property);
            }
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Description"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Description";
                context.TicketHistoryProperties.Add(property);
            }
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Type"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Type";
                context.TicketHistoryProperties.Add(property);
            }
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Priority"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Priority";
                context.TicketHistoryProperties.Add(property);
            }
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Status"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Status";
                context.TicketHistoryProperties.Add(property);
            }
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Asignee"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Asignee";
                context.TicketHistoryProperties.Add(property);
            }
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Attachment"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Attachment";
                context.TicketHistoryProperties.Add(property);
            }
            if (!context.TicketHistoryProperties.Any(p => p.Name == "Comment"))
            {
                var property = new TicketHistoryProperty();
                property.Name = "Comment";
                context.TicketHistoryProperties.Add(property);
            }

            //Ticket Priorities
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

            //Ticket Statuses
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
            if (!context.TicketStatuses.Any(p => p.Name == "Resolved"))
            {
                var status = new TicketStatus();
                status.Name = "Resolved";
                context.TicketStatuses.Add(status);
            }

            //Ticket Types
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

            //User Roles
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
            
            //New user
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

            //New user
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

            //New user
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

            //New user
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

            //New user
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

            //New user
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

            //New user
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

            //New user
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
