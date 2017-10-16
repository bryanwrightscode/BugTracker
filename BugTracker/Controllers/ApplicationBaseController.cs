
using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                    if (Request.IsAuthenticated)
                    {
                        baseModel.CurrentUser = db.Users.Find(User.Identity.GetUserId());
                        baseModel.IsNotificationHistories = baseModel.CurrentUser.Histories.Where(h => h.IsNotification == true && h.DeveloperId == baseModel.CurrentUser.Id).OrderByDescending(h => h.Created).ToArray();
                    }
                }
            }
        }
    }
}