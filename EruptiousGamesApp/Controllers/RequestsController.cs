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

        //Delete
        //// GET: Requests
        //public ActionResult Index()
        //{
        //    var requests = db.Requests.Include(r => r.Campaign).Include(r => r.Employee);
        //    return View(requests.ToList());
        //}
        //Delete

        // GET: Requests/RequestAdmin
        public ActionResult RequestAdmin()
        {
            var requests = db.Requests.Include(r => r.Campaign).Include(r => r.Employee);
            var employees = db.Employees.ToList();
            ViewBag.employeeList = employees;
            return View(requests.ToList());
        }

        //Delete
        //// GET: Requests/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Request request = db.Requests.Find(id);
        //    if (request == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(request);
        //}


        //// GET: Requests/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName");
        //    ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName");
        //    return View();
        //}
        //Delete


        // GET: Requests/InputRequest
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

        //Delete
        //// POST: Requests/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "RequestID,CamID,EmpID,DateTime,Amount,Action,RequestStatus")] Request request)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        db.Requests.Add(request);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", request.CamID);
        //    ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", request.EmpID);
        //    return View(request);
        //}
        //Delete

        // POST: Requests/InputRequest
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InputRequest([Bind(Include = "RequestID,CamID,EmpID,DateTime,Amount,Action,RequestStatus")] Request request)
        {
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            request.CamID = currentUser.GetTodaysCampaign().CamID;
            request.EmpID = currentUser.Employee.EmpID;
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

        //Delete
        //// GET: Requests/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Request request = db.Requests.Find(id);
        //    if (request == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", request.CamID);
        //    ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", request.EmpID);
        //    return View(request);
        //}

        //// POST: Requests/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "RequestID,CamID,EmpID,DateTime,Amount,Action,RequestStatus")] Request request)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(request).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", request.CamID);
        //    ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", request.EmpID);
        //    return View(request);
        //}
        //Delete

        // GET: Requests/Acept/5
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

        //Delete
        //// GET: Requests/ChangeStatus/5
        //public ActionResult ChangeStatus(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Request request = db.Requests.Find(id);
        //    if (request == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", request.CamID);
        //    ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", request.EmpID);

        //    return View(request);
        //}

        //// GET: Requests/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Request request = db.Requests.Find(id);
        //    if (request == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(request);
        //}

        //// POST: Requests/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Request request = db.Requests.Find(id);
        //    db.Requests.Remove(request);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        //Delete

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
