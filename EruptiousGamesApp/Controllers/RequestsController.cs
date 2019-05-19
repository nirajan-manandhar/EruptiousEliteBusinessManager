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
using Microsoft.AspNet.Identity.Owin;

namespace EruptiousGamesApp.Controllers
{
    public class RequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class EmployeeRequest
        {
            public Request request;
            public Employee employee;
        }

        // GET: Requests/RequestAdmin
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult RequestAdmin()
        {
            //var max = DateTime.Now.AddDays(1);
            //var min = DateTime.Now.AddDays(-1);
            //var requests = db.Requests.Include(r => r.Campaign).Include(r => r.Employee).OrderBy(r => r.DateTime).OrderBy(r => r.Campaign.CamName).Where(r => r.DateTime <= max).Where(r => r.DateTime >= min);

            var requests = db.Requests.Include(r => r.Campaign).Include(r => r.Employee).OrderBy(r => r.DateTime).OrderBy(r => r.Campaign.CamName);
            var employees = db.Employees.ToList();
            ViewBag.employeeList = employees;
            return View(requests.ToList());
        }


        // GET: Requests/InputRequest
        [AuthorizeUser(Role = Role.AMBASSADOR)]
        public ActionResult InputRequest()
        {
            EmployeeRequest empReq = new EmployeeRequest();
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            empReq.employee = currentUser.Employee;
            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName");
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName");
            return View(empReq);
        }


        // POST: Requests/InputRequest
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Role = Role.AMBASSADOR)]
        public ActionResult InputRequest([Bind(Include = "RequestID,CamID,EmpID,DateTime,Amount,Action,RequestStatus")] Request request)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            request.CamID = currentUser.GetTodaysCampaign().CamID;
            request.EmpID = currentUser.Employee.EmpID;

            if ((int)request.Action == 0)
            {
                if (request.Amount > currentUser.GetTodaysCampaign().Inventory)
                {
                    TempData["error"] = "The amount requested exceeds the number of inventory available.";
                    return RedirectToAction("InputRequest");
                }
            }
            else
            {
                if (request.Amount > currentUser.Employee.DecksOnHand)
                {
                    TempData["error"] = "You are attempting to return more decks than you have on hand.";
                    return RedirectToAction("InputRequest");
                }
            }


            if (ModelState.IsValid)
            {
                db.Requests.Add(request);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", request.CamID);
            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", request.EmpID);
            //return View(request);
            return RedirectToAction("Index", "Home");
        }

        // GET: Requests/Acept/5
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }


            request.RequestStatus = RequestStatus.APPROVED;
            if (request.Action == Entities.Action.REQUEST)
            {
                request.Employee.DecksOnHand += request.Amount;
                request.Campaign.Inventory -= request.Amount;
            }
            else
            {
                request.Employee.DecksOnHand -= request.Amount;
                request.Campaign.Inventory += request.Amount;

            }

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            request.EmpID = currentUser.Employee.EmpID;

            if (ModelState.IsValid)
            {
                //db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RequestAdmin");
            }

            return RedirectToAction("RequestAdmin");
        }

        // GET: Requests/Decline/5
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult Deny(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }

            request.RequestStatus = RequestStatus.DENIAL;

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            request.EmpID = currentUser.Employee.EmpID;

            if (ModelState.IsValid)
            {
                //db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("RequestAdmin");
            }
            return RedirectToAction("RequestAdmin");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
