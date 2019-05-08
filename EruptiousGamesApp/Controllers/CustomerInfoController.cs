using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EruptiousGamesApp.Entities;
using EruptiousGamesApp.Models;

namespace EruptiousGamesApp.Controllers
{
    public class CustomerInfoController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerInfo
        public ActionResult Index()
        {
            var customers = db.Customers.Include(c => c.Campaign).Include(c => c.Employee);
            return View(customers.ToList());
        }
    }
}