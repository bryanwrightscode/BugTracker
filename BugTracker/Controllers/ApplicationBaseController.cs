using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class ApplicationBaseController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (User != null)
            {
                //var context = new ApplicationDbContext();
                //var username = User.Identity.Name;

                //if (!string.IsNullOrEmpty(username))
                //{
                //    var user = context.Users.SingleOrDefault(u => u.UserName == username);

                //    string firstName = "abc";
                //    ViewData.Add("FirstName", firstName);
                //}

                if (Request.IsAuthenticated)
                {
                    var user = db.Users.Find(User.Identity.GetUserId());
                    string firstName = user.FirstName;
                    string lastName = user.LastName;
                    string fullName = user.FullName;
                    ViewData.Add("FirstName", firstName);
                    ViewData.Add("LastName", lastName);
                    ViewData.Add("FullName", fullName);

                }
            }
            base.OnActionExecuted(filterContext);
        }
        public ApplicationBaseController()
        { }
    }
}