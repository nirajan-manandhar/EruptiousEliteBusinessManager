using EruptiousGamesApp.Entities;
using EruptiousGamesApp.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EruptiousGamesApp.Authorization
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // Custom property
        public Role Role { get; set; }

        private ApplicationDbContext db = new ApplicationDbContext();

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            Role CurrentRole = GetUserRole(httpContext.User.Identity.Name.ToString());

            EmpStatus CurrentEmpStatus = GetUserEmpStatus(httpContext.User.Identity.Name.ToString());

            return CurrentRole >= this.Role && CurrentEmpStatus == EmpStatus.ACTIVE;
        }

        private Role GetUserRole(string UserName)
        {
            ApplicationUser currentUser = db.Users.Include(r => r.Employee).FirstOrDefault(x => x.UserName == UserName);
            return currentUser.Employee.Role;
        }

        private EmpStatus GetUserEmpStatus(string UserName)
        {
            ApplicationUser currentUser = db.Users.Include(r => r.Employee).FirstOrDefault(x => x.UserName == UserName);
            return currentUser.Employee.EmpStatus;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Home",
                                action = "Unauthorised"
                            })
                        );
        }

    }
}