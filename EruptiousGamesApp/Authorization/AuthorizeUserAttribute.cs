using EruptiousGamesApp.Entities;
using EruptiousGamesApp.Models;
using System;
using System.Collections.Generic;
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

            return CurrentRole == this.Role;
        }

        private Role GetUserRole(string UserName)
        {
            ApplicationUser currentUser = db.Users.Include(r => r.Employee).FirstOrDefault(x => x.UserName == UserName);
            return currentUser.Employee.Role;
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