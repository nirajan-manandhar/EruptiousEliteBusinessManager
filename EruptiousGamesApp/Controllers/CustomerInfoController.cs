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

        // GET: Customer/Index
        public ActionResult Index()
        {
            return RedirectToAction("Create", "CustomerInfo");
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName");
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName");
            return View();
        }

        //POST: CustomerInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustID,CamID,EmpID,DateTime,CustName,Email,Phone,City,Age,Gender,PTCheck")] Customer customer)
        {

            customer.CamID = 1;
            customer.EmpID = 1;
            customer.DateTime = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", customer.CamID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", customer.EmpID);

            return View(customer);
        }
    }
}