using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EruptiousGamesApp.Authorization;
using EruptiousGamesApp.Entities;
using EruptiousGamesApp.Models;
using Microsoft.AspNet.Identity;

namespace EruptiousGamesApp.Controllers
{
    public class CustomerInfoController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customers/Create
        [AuthorizeUser(Role = Role.AMBASSADOR)]
        public ActionResult Create()
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));
            var todayCam = currentUser.GetTodaysCampaign();
            if (todayCam == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName");
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName");
            return View();
        }

        //POST: CustomerInfo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Role = Role.AMBASSADOR)]
        public ActionResult Create([Bind(Include = "CustID,CamID,EmpID,DateTime,CustName,Email,Phone,City,Age,Gender,PTCheck")] Customer customer, int morePeople)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            customer.CamID = currentUser.GetTodaysCampaign().CamID;
            customer.EmpID = currentUser.Employee.EmpID;
            customer.DateTime = DateTime.Now;

            //Encrypt
            customer.CustName = Encryptor.Encrypt(customer.CustName);
            customer.Email = Encryptor.Encrypt(customer.Email);
            if (customer.Phone != null)
            {
                customer.Phone = Encryptor.Encrypt(customer.Phone);
            }

            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();

                if (morePeople == 0)
                {
                    return RedirectToAction("Create", "Notes");
                }
                else if (morePeople == 1) 
                {
                    return RedirectToAction("Create");
                }
            } 

            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", customer.CamID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", customer.EmpID);
            return View(customer);
        }
    }
}