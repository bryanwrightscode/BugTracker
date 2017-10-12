using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class ApplicationBaseController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var result = filterContext.Result as ViewResult;
            if (result != null)
            {
                var baseModel = result.Model as ApplicationBaseViewModel;
                if (baseModel != null)
                {
                    baseModel.CurrentUser = db.Users.Find(User.Identity.GetUserId());
                    baseModel.IsNotificationHistories = baseModel.CurrentUser.Histories.Where(h => h.IsNotification == true && h.DeveloperId == baseModel.CurrentUser.Id).OrderByDescending(h => h.Created).Take(10).ToArray();
                }
            }
        }
    }
}