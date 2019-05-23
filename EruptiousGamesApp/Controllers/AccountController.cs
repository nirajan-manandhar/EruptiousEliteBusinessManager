using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EruptiousGamesApp.Models;
using EruptiousGamesApp.Entities;
using System.Data.Entity;
using System.Net;
using EruptiousGamesApp.Authorization;


namespace EruptiousGamesApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = db.Users.Include(u => u.Employee).FirstOrDefault(x => x.UserName == model.UserName);
            if (currentUser != null)
            {
                if (currentUser.Employee.EmpStatus == EmpStatus.INACTIVE)
                {
                    ModelState.AddModelError("", "Inactive Account.");
                    return View(model);
                }
            }


            SignInStatus result;
            result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        // GET: /Account/AccountCreate
        [AuthorizeUser(Role = Role.ADMIN)]
        public ActionResult AccountCreate()
        {
            return View();
        }

        // POST: /Account/AccountCreaet
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Role = Role.ADMIN)]
        public async Task<ActionResult> AccountCreate(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Employee = new Employee { EmpName = model.Employee.EmpName, Role = model.Employee.Role, EmpStatus = model.Employee.EmpStatus, DecksOnHand = 0 } };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("AccountList", "Account");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/AccountList
        [AuthorizeUser(Role = Role.ADMIN)]
        public ActionResult AccountList()
        {
            var users = db.Users.Include(r => r.Employee).Include(r => r.Employee.Workings).OrderBy(r => r.Employee.EmpName);
            return View(users.ToList());
        }

        // GET: Account/AccountEdit/5
        [AuthorizeUser(Role = Role.ADMIN)]
        public ActionResult AccountEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser ApplicationUser = db.Users.Include(r => r.Employee).FirstOrDefault(x => x.Id == id);
            
            //Protect Superuser - Terence's account
            if (ApplicationUser.UserName == "terence")
            {
                return RedirectToAction("Unauthorised", "Home");
            }

            if (ApplicationUser == null)
            {
                return HttpNotFound();
            }

            return View(ApplicationUser);
        }

        // POST: Account/AccountEdit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Role = Role.ADMIN)]
        public async Task<ActionResult> AccountEdit([Bind(Include = "Id,UserName,Employee")] ApplicationUser ApplicationUser)
        {
            if (ModelState.IsValid)
            {
                var user = UserManager.FindById(ApplicationUser.Id);
                //Protect Superuser - Terence's account
                if (user.UserName == "terence")
                {
                    return RedirectToAction("Unauthorised", "Home");
                }


                user.UserName = ApplicationUser.UserName;



                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    db.Entry(ApplicationUser.Employee).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AccountList", "Account");
                }
                AddErrors(result);

                return View(ApplicationUser);
            }

            return View(ApplicationUser);
        }

        // GET: Account/AccountChangePassword/5
        [AuthorizeUser(Role = Role.ADMIN)]
        public ActionResult AccountChangePassword(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser ApplicationUser = db.Users.Include(r => r.Employee).FirstOrDefault(x => x.Id == id);

            //Protect Superuser - Terence's account
            if (ApplicationUser.UserName == "terence")
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

                if (currentUser.UserName != "terence")
                {
                    return RedirectToAction("Unauthorised", "Home");
                }

            }

            if (ApplicationUser == null)
            {
                return HttpNotFound();
            }

            ChangePasswordModel model = new ChangePasswordModel();
            model.UserId = ApplicationUser.Id;

            return View(model);
        }

        // POST: Account/AccountEdit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Role = Role.ADMIN)]
        public async Task<ActionResult> AccountChangePassword([Bind(Include = "UserId,Password,ConfirmPassword")]ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                //Protect Superuser - Terence's account
                ApplicationUser ApplicationUser = db.Users.FirstOrDefault(x => x.Id == model.UserId);
                if (ApplicationUser.UserName == "terence")
                {
                    string currentUserId = User.Identity.GetUserId();
                    ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);

                    if (currentUser.UserName != "terence")
                    {
                        return RedirectToAction("Unauthorised", "Home");
                    }

                }


                var result = UserManager.RemovePassword(model.UserId);
                
                if (result.Succeeded)
                {
                    var result2 = await UserManager.AddPasswordAsync(model.UserId, model.Password);
                    if (result2.Succeeded)
                    {
                        return RedirectToAction("AccountList");
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
                
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}