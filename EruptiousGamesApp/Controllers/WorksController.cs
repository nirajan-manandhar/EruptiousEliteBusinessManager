//Delete all
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using EruptiousGamesApp.Entities;
//using EruptiousGamesApp.Models;

//namespace EruptiousGamesApp.Controllers
//{
//    public class WorksController : Controller
//    {
//        private ApplicationDbContext db = new ApplicationDbContext();

//        // GET: Works
//        public ActionResult Index()
//        {
//            var works = db.Works.Include(w => w.Campaign).Include(w => w.Employee);
//            return View(works.ToList());
//        }

//        // GET: Works/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Work work = db.Works.Find(id);
//            if (work == null)
//            {
//                return HttpNotFound();
//            }
//            return View(work);
//        }

//        // GET: Works/Create
//        public ActionResult Create()
//        {
//            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName");
//            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName");
//            return View();
//        }

//        // POST: Works/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "WorkID,CamID,EmpID,Date,Sold,CustomerPlayWith")] Work work)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Works.Add(work);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", work.CamID);
//            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", work.EmpID);
//            return View(work);
//        }

//        // GET: Works/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Work work = db.Works.Find(id);
//            if (work == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", work.CamID);
//            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", work.EmpID);
//            return View(work);
//        }

//        // POST: Works/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "WorkID,CamID,EmpID,Date,Sold,CustomerPlayWith")] Work work)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(work).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.CamID = new SelectList(db.Campaigns, "CamID", "CamName", work.CamID);
//            ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", work.EmpID);
//            return View(work);
//        }

//        // GET: Works/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Work work = db.Works.Find(id);
//            if (work == null)
//            {
//                return HttpNotFound();
//            }
//            return View(work);
//        }

//        // POST: Works/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            Work work = db.Works.Find(id);
//            db.Works.Remove(work);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
