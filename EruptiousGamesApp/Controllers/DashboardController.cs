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

        public class DashBoardItem{
            public int personalSalePerDay { get; set; }
            public int personalPlayedPerDay { get; set; }
            public double personalClosingRatioPerDay { get; set; }

            public int personalSalePerCampaign { get; set; }
            public int personalPlayedPerCampaign { get; set; }
            public double personalClosingRatioPerCampaign { get; set; }

            public Boolean assignedToCampaign { get; set; }

            public int overallSale { get; set; }
            public int overallPlayed { get; set; }
            public double overallClosingRatio { get; set; }

            public IEnumerable<string> campaignNames { get; set; }
            public IEnumerable<int> campaignSales { get; set; }
            public IEnumerable<int> campaignPlayeds { get; set; }
            public IEnumerable<double> campaignClosingRatioes { get; set; }

            public Employee employee;
        }

        public ActionResult Index()
        {
            DashBoardItem di = new DashBoardItem();

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));


            int empID = currentUser.Employee.EmpID;
            di.employee = currentUser.Employee;


            //Ambassdor per day
            var work = db.Works.Where(s => s.EmpID == empID && s.Date == DateTime.Today);
            if (work.Count() > 0)
            {
                di.personalPlayedPerDay = work.Sum(s => s.CustomerPlayWith);
                di.personalSalePerDay = work.Sum(s => s.Sold);
                di.personalClosingRatioPerDay = getClosingRatio(di.personalSalePerDay, di.personalPlayedPerDay);
            }
            else {
                di.personalPlayedPerDay = 0;
                di.personalSalePerDay = 0;
                di.personalClosingRatioPerCampaign = 0;
            }

            //Ambassdor per campaign
            var todayCam = currentUser.GetTodaysCampaign();
            if (todayCam != null) {
                di.assignedToCampaign = true;
                work = db.Works.Where(s => s.EmpID == empID && s.CamID == todayCam.CamID);
                if (work.Count() > 0)
                {
                    di.personalPlayedPerCampaign = work.Sum(s => s.CustomerPlayWith);
                    di.personalSalePerCampaign = work.Sum(s => s.Sold);
                    di.personalClosingRatioPerCampaign = getClosingRatio(di.personalSalePerCampaign, di.personalPlayedPerCampaign);
                }
                else
                {
                    di.personalPlayedPerCampaign = 0;
                    di.personalSalePerCampaign = 0;
                    di.personalClosingRatioPerCampaign = 0;
                }
            }
            else
            {
                di.assignedToCampaign = false;
                di.personalPlayedPerCampaign = 0;
                di.personalSalePerCampaign = 0;
                di.personalClosingRatioPerCampaign = 0;
            }

            //Manager Overall
            work = db.Works;
            if (work.Count() > 0)
            {
                di.overallPlayed = work.Sum(s => s.CustomerPlayWith);
                di.overallSale = work.Sum(s => s.Sold);
                di.overallClosingRatio = getClosingRatio(di.overallSale, di.overallPlayed);
            }
            else
            {
                di.overallPlayed = 0;
                di.overallSale = 0;
                di.overallClosingRatio = 0;
            }

            //Manager list of campaigns
            var campaigns = db.Campaigns.ToList();

            List<string> campaignNames = new List<string>();
            List<int> campaignSales = new List<int>();
            List<int> campaignPlayeds = new List<int>();
            List<double> campaignClosingRatioes = new List<double>();

            foreach (var c in campaigns) {
                campaignNames.Add(c.CamName);

                var cam = db.Works.Where(s => s.CamID == c.CamID);
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
            di.campaignSales = campaignSales;
            di.campaignPlayeds = campaignPlayeds;
            di.campaignClosingRatioes = campaignClosingRatioes;

            return View(di);
        }

        private double getClosingRatio(int sale, int played) {
            return Math.Round(((Convert.ToDouble(sale) / Convert.ToDouble(played)) * 100), 2);
        }
    }
}