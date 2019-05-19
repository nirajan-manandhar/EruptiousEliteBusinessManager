using EruptiousGamesApp.Authorization;
using EruptiousGamesApp.Entities;
using EruptiousGamesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EruptiousGamesApp.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class DashBoardItem{
            public int personalSale { get; set; }
            public int personalPlayed { get; set; }
            public int personalInfoCollected { get; set; }

            public int campaignSale { get; set; }
            public int campaignPlayed { get; set; }
            public int campaignInfoColleted { get; set; }

            public int totalAccount { get; set; }
        }

        [AuthorizeUser(Role = Role.AMBASSADOR)]
        public ActionResult Index()
        {
            DashBoardItem di = new DashBoardItem();

            int testEmpId = 5;

            var work = db.Works.Where(s => s.EmpID == testEmpId);
            if (work.Count() > 0)
            {
                di.personalPlayed = work.Sum(s => s.CustomerPlayWith);
                di.personalSale = work.Sum(s => s.Sold);
            }
            else {
                di.personalPlayed = 0;
                di.personalSale = 0;
            }
            var customer = db.Customers.Where(s => s.EmpID == testEmpId);
            if (customer.Count() > 0)
            {
                di.personalInfoCollected = customer.Count();
            }
            else {
                di.personalInfoCollected = 0;
            }

            int campaignId = 1;
            var campaign = db.Works.Where(s => s.CamID == campaignId);
            if(campaign.Count() > 0) { 
                di.campaignPlayed = campaign.Sum(s => s.CustomerPlayWith);
                di.campaignSale = campaign.Sum(s => s.Sold);
            }
            customer = db.Customers.Where(s => s.CamID == campaignId);
            if (customer.Count() > 0)
            {
                di.campaignInfoColleted = customer.Count();
            }
            else
            {
                di.campaignInfoColleted = 0;
            }

            di.totalAccount = db.Employees.Count();

            return View(di);
        }

        public ActionResult Unauthorised()
        {
            ViewBag.Message = "Sorry, you are not Authorized to access this page.";
            return View();
        }
    }
}