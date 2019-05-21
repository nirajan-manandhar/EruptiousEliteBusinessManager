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


        // GET: Notes/Create
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

            WorkSession ws = new WorkSession();
            return View(ws);
        }

        // POST: Notes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Role = Role.AMBASSADOR)]
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

                var existingWork = db.Works.Where(s => s.Date == work.Date && s.EmpID == work.EmpID && s.CamID == work.CamID);

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

            if (!String.IsNullOrWhiteSpace(note.Title) || !String.IsNullOrWhiteSpace(note.Comment))
            {
                db.Notes.Add(note);
            }

            db.SaveChanges();

            return Redirect("/home");
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
