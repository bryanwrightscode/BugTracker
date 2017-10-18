
using BugTracker.Models;
using BugTracker.Models.CodeFirst;
using BugTracker.Models.Helpers;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
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
        UserRoleHelper helper = new UserRoleHelper();
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
                        baseModel.IsNotificationHistories = baseModel.CurrentUser.Histories.Where(h => h.IsNotification == true && h.IsAlertArchived == false && h.DeveloperId == baseModel.CurrentUser.Id).OrderByDescending(h => h.Created).ToArray();
                        baseModel.Role = helper.ListUserRoles(baseModel.CurrentUser.Id).ToArray();
                    }
                }
            }
        }

        public ActionResult MarkAsRead()
        {
            ApplicationBaseViewModel vm = new ApplicationBaseViewModel();
            vm.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            var alerts = vm.CurrentUser.Histories.Where(h => h.IsNotification == true && h.IsAlertArchived == false && h.IsClicked == false && h.DeveloperId == vm.CurrentUser.Id).OrderByDescending(h => h.Created);
            foreach (var alert in alerts)
            {
                alert.IsClicked = true;
                db.Entry(alert).State = EntityState.Modified;
            }
            db.SaveChanges();
            vm.IsNotificationHistories = alerts.ToArray();
            vm.Role = helper.ListUserRoles(vm.CurrentUser.Id).ToArray();
            return RedirectToLocal(Request.UrlReferrer.ToString());
        }

        public ActionResult Clear()
        {
            ApplicationBaseViewModel vm = new ApplicationBaseViewModel();
            vm.CurrentUser = db.Users.Find(User.Identity.GetUserId());
            var alerts = vm.CurrentUser.Histories.Where(h => h.IsNotification == true && h.IsAlertArchived == false && h.DeveloperId == vm.CurrentUser.Id).OrderByDescending(h => h.Created);
            foreach (var alert in alerts)
            {
                alert.IsAlertArchived = true;
                db.Entry(alert).State = EntityState.Modified;
            }
            db.SaveChanges();
            vm.IsNotificationHistories = alerts.ToArray();
            vm.Role = helper.ListUserRoles(vm.CurrentUser.Id).ToArray();
            return RedirectToLocal(Request.UrlReferrer.ToString());
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}