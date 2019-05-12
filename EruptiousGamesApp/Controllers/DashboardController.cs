using EruptiousGamesApp.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EruptiousGamesApp.Controllers
{
    public class DashboardController : Controller
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

        public ActionResult Index()
        {
            DashBoardItem di = new DashBoardItem();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            int empID = currentUser.Employee.EmpID;

            var work = db.Works.Where(s => s.EmpID == empID);
            if (work.Count() > 0)
            {
                di.personalPlayed = work.Sum(s => s.CustomerPlayWith);
                di.personalSale = work.Sum(s => s.Sold);
            }
            else {
                di.personalPlayed = 0;
                di.personalSale = 0;
            }
            var customer = db.Customers.Where(s => s.EmpID == empID);
            if (customer.Count() > 0)
            {
                di.personalInfoCollected = customer.Count();
            }
            else {
                di.personalInfoCollected = 0;
            }

            var todayCam = currentUser.GetTodaysCampaign();

            if (todayCam == null)
            {
                di.campaignPlayed = 0;
                di.campaignSale = 0;
                di.campaignInfoColleted = 0;
            }
            else {
                int campaignId = todayCam.CamID;
                var campaign = db.Works.Where(s => s.CamID == campaignId);
                if (campaign.Count() > 0)
                {
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
            }

            di.totalAccount = db.Employees.Count();

            return View(di);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}