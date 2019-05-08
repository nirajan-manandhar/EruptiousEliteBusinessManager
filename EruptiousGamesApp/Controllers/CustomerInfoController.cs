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
            //ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName");
            //ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName");

            ViewBag.CamID = 1;
            ViewBag.EmpID = 1;

            return View();
        }

        //POST: CustomerInfo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustID,CamID,EmpID,DateTime,CustName,Email,Phone,City,Age,Gender,PTCheck")] Customer customer)
        {

            var dummyCamID = 1;
            var dummyEmpID = 1;
            var dummyDateTime = DateTime.Now;

            customer.CamID = dummyCamID;
            customer.EmpID = dummyEmpID;
            customer.DateTime = dummyDateTime;

            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", customer.CamID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", customer.EmpID);
            return View(customer);
        }
    }
}