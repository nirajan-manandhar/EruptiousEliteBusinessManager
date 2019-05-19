using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
    public class NotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public class WorkSession {
            public Note note;
            public Work work;
        }
        public class Notes
        {
            public IEnumerable<Note> notes;
            public Employee emp;
        }

        // GET: Notes
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult Index()
        {
            var notes = db.Notes.Include(n => n.Employee);
            Notes nn = new Notes();
            nn.notes = notes;
            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));
            nn.emp = currentUser.Employee;

            return View(nn);
        }

        // POST: Notes/DeleteNote
        [AuthorizeUser(Role = Role.ADMIN)]
        [HttpPost, ActionName("Index")]
        public ActionResult DeleteNote(string noteID)
        {
            Note note = db.Notes.Find(Int32.Parse(noteID));
            db.Notes.Remove(note);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Delete
        //// GET: Notes/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Note note = db.Notes.Find(id);
        //    if (note == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(note);
        //}
        //Delete

        // GET: Notes/Create
        public ActionResult Create()
        {
            WorkSession ws = new WorkSession();
            return View(ws);
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NoteID,EmpID,DateTime,Title,Comment")] Note note, [Bind(Include = "CustomerPlayWith, Sold")] Work work)
        {
            if (!ModelState.IsValid){
                WorkSession ws = new WorkSession
                {
                    note = note,
                    work = work
                };
                return View(ws);
            }

            string currentUserId = User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            int empID = currentUser.Employee.EmpID;
            var currentCam = currentUser.GetTodaysCampaign();

            note.EmpID = empID;
            work.EmpID = empID;

            if (currentCam == null)
            {
                work.CamID = 0;
            }
            else {
                int camID = currentCam.CamID;
                work.CamID = camID;

                var existingWork = db.Works.Where(s => s.Date == work.Date && s.EmpID == work.EmpID);

                if (existingWork.Count() > 0)
                {
                    db.Entry(existingWork.First()).State = EntityState.Modified;
                    existingWork.First().CustomerPlayWith += work.CustomerPlayWith;
                    existingWork.First().Sold += work.Sold;
                }
                else
                {
                    db.Works.Add(work);
                }
                db.Entry(currentUser.Employee).State = EntityState.Modified;
                currentUser.Employee.DecksOnHand -= work.Sold;
            }

            if (String.IsNullOrWhiteSpace(note.Title) || String.IsNullOrWhiteSpace(note.Comment))
            {
                TempData["error"] = "A title or comment is required.";
                return RedirectToAction("Create");
            } else
            {
                db.Notes.Add(note);
            }

            /*
            if (!String.IsNullOrWhiteSpace(note.Title) || !String.IsNullOrWhiteSpace(note.Comment))
            {
                db.Notes.Add(note);
            }
            */
            db.Configuration.ValidateOnSaveEnabled = false;
            db.SaveChanges();

            return Redirect("/home");
        }

        //Delete
        //// GET: Notes/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Note note = db.Notes.Find(id);
        //    if (note == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    //ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", note.EmpID);
        //    return View(note);
        //}

        //// POST: Notes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "NoteID,EmpID,DateTime,Title,Comment")] Note note)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(note).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    //ViewBag.EmpID = new SelectList(db.Employees, "EmpID", "EmpName", note.EmpID);
        //    return View(note);
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
