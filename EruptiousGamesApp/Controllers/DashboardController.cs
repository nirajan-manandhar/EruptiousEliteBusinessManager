using EruptiousGamesApp.Authorization;
using EruptiousGamesApp.Entities;
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

        public class EmployeeStat {
            public string name { get; set; }
            public Role role { get; set; }
            public int decks { get; set; }
            public bool assignedToCampaign { get; set; }

            public int salePerDay { get; set; }
            public int playedPerDay { get; set; }
            public double closingRatioPerDay { get; set; }

            public int salePerCampaign { get; set; }
            public int playedPerCampaign { get; set; }
            public double closingRatioPerCampaign { get; set; }

            public int saleOverall { get; set; }
            public int playedOverall { get; set; }
            public double closingRatioOverall { get; set; }
        }

        public class DashBoardItem {
            public DashBoardItem(){
                personalStat = new EmployeeStat();
            }
            public EmployeeStat personalStat { get; set; }
            public int overallSale { get; set; }
            public int overallPlayed { get; set; }
            public double overallClosingRatio { get; set; }

            public IEnumerable<string> campaignNames { get; set; }
            public IEnumerable<int> campaignInventory { get; set; }
            public IEnumerable<int> campaignSales { get; set; }
            public IEnumerable<int> campaignPlayeds { get; set; }
            public IEnumerable<double> campaignClosingRatioes { get; set; }
            public IEnumerable<EmployeeStat> employeeStats { get; set; }
        }

        [AuthorizeUser(Role = Role.AMBASSADOR)]
        public ActionResult Index()
        {
            DashBoardItem di = new DashBoardItem();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));


            int empID = currentUser.Employee.EmpID;
            di.personalStat.role = currentUser.Employee.Role;
            di.personalStat.name = currentUser.Employee.EmpName;
            di.personalStat.decks = currentUser.Employee.DecksOnHand;

            //Ambassdor per day
            var work = db.Works.Where(s => s.EmpID == empID && s.Date == DateTime.Today);
            if (work.Count() > 0)
            {
                di.personalStat.playedPerDay = work.Sum(s => s.CustomerPlayWith);
                di.personalStat.salePerDay = work.Sum(s => s.Sold);
                di.personalStat.closingRatioPerDay = getClosingRatio(di.personalStat.salePerDay, di.personalStat.playedPerDay);
            }

            //Ambassdor per campaign
            var todayCam = currentUser.GetTodaysCampaign();
            if (todayCam != null)
            {
                di.personalStat.assignedToCampaign = true;
                work = db.Works.Where(s => s.EmpID == empID && s.CamID == todayCam.CamID);
                if (work.Count() > 0)
                {
                    di.personalStat.playedPerCampaign = work.Sum(s => s.CustomerPlayWith);
                    di.personalStat.salePerCampaign = work.Sum(s => s.Sold);
                    di.personalStat.closingRatioPerCampaign = getClosingRatio(di.personalStat.salePerCampaign, di.personalStat.playedPerCampaign);
                }
            }

            //Ambassdor overall
            work = db.Works.Where(s => s.EmpID == empID);
            if (work.Count() > 0)
            {
                di.personalStat.playedOverall = work.Sum(s => s.CustomerPlayWith);
                di.personalStat.saleOverall = work.Sum(s => s.Sold);
                di.personalStat.closingRatioOverall = getClosingRatio(di.personalStat.saleOverall, di.personalStat.playedOverall);
            }

            //Exit for ambassdor
            if (currentUser.Employee.Role == Role.AMBASSADOR) {
                return View(di);
            }

            //Manager Overall
            var list = db.Works.ToList();
            if (list.Count() > 0)
            {
                di.overallPlayed = list.Sum(s => s.CustomerPlayWith);
                di.overallSale = list.Sum(s => s.Sold);
                di.overallClosingRatio = getClosingRatio(di.overallSale, di.overallPlayed);
            }

            //Manager list of campaigns
            var campaigns = db.Campaigns.ToList();

            List<string> campaignNames = new List<string>();
            List<int> campaignInventory = new List<int>();
            List<int> campaignSales = new List<int>();
            List<int> campaignPlayeds = new List<int>();
            List<double> campaignClosingRatioes = new List<double>();

            foreach (var c in campaigns) {
                campaignNames.Add(c.CamName);
                campaignInventory.Add(c.Inventory);

                var cam = db.Works.Where(s =>  s.CamID == c.CamID).ToList();
                if (cam.Count() > 0)
                {
                    campaignPlayeds.Add(cam.Sum(s => s.CustomerPlayWith));
                    campaignSales.Add(cam.Sum(s => s.Sold));
                    campaignClosingRatioes.Add(getClosingRatio(campaignSales.Last(), campaignPlayeds.Last()));
                }
                else {
                    campaignPlayeds.Add(0);
                    campaignSales.Add(0);
                    campaignClosingRatioes.Add(0);
                }
            }
            di.campaignNames = campaignNames;
            di.campaignInventory = campaignInventory;
            di.campaignSales = campaignSales;
            di.campaignPlayeds = campaignPlayeds;
            di.campaignClosingRatioes = campaignClosingRatioes;

            //Exit for manager
            if (currentUser.Employee.Role == Role.MANAGER)
            {
                return View(di);
            }

            //Employee list for Admin
            
            var employees = db.Employees.ToList();
            var works = db.Works.ToList();
            var account = db.Users.ToList();

            List<EmployeeStat> employeeStats = new List<EmployeeStat>();

            foreach (var a in account)
            {
                var e = a.Employee;
                EmployeeStat es = new EmployeeStat();
                es.name = e.EmpName;
                es.role = e.Role;
                es.decks = e.DecksOnHand;

                //Ambassdor per day
                work = db.Works.Where(s => s.EmpID == e.EmpID && s.Date == DateTime.Today);
                if (work.Count() > 0)
                {
                    es.playedPerDay = work.Sum(s => s.CustomerPlayWith);
                    es.salePerDay = work.Sum(s => s.Sold);
                    es.closingRatioPerDay = getClosingRatio(es.salePerDay, es.playedPerDay);
                }

                //Ambassdor per campaign
                todayCam = e.GetTodaysCampaign();
                if (todayCam != null)
                {
                    es.assignedToCampaign = true;
                    work = db.Works.Where(s => s.EmpID == e.EmpID && s.CamID == todayCam.CamID);
                    if (work.Count() > 0)
                    {
                        es.playedPerCampaign = work.Sum(s => s.CustomerPlayWith);
                        es.salePerCampaign = work.Sum(s => s.Sold);
                        es.closingRatioPerCampaign = getClosingRatio(es.salePerCampaign, es.playedPerCampaign);
                    }
                }

                //Ambassdor overall
                work = db.Works.Where(s => s.EmpID == e.EmpID);
                if (work.Count() > 0)
                {
                    es.playedOverall = work.Sum(s => s.CustomerPlayWith);
                    es.saleOverall = work.Sum(s => s.Sold);
                    es.closingRatioOverall = getClosingRatio(es.saleOverall, es.playedOverall);
                }
                employeeStats.Add(es);
            }
            di.employeeStats = employeeStats;

            //Exit for admin
            return View(di);
        }

        private double getClosingRatio(int sale, int played) {
            if (played == 0) {
                return 0;
            }
            return Math.Round(((Convert.ToDouble(sale) / Convert.ToDouble(played)) * 100), 2);
        }
    }
}