using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EruptiousGamesApp.Authorization;
using EruptiousGamesApp.Entities;
using EruptiousGamesApp.Models;
using OfficeOpenXml;

namespace EruptiousGamesApp.Controllers
{
    public class CampaignsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Campaigns
        public ActionResult Index()
        {
            return View(db.Campaigns.ToList());
        }

        // GET: Campaigns/Create
        public ActionResult Create()
        {
            return PartialView("Create", new Campaign());
        }

        // POST: Campaigns/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CamID,CamName,StartDate,EndDate,Inventory")] Campaign campaign)
        {

            Debug.WriteLine(campaign.StartDate);
            Debug.WriteLine(campaign.EndDate);

            if (ModelState.IsValid)
            {
                db.Campaigns.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campaign);
        }

        // GET: Campaigns/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // GET: Campaigns/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // POST: Campaigns/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CamID,CamName,StartDate,EndDate,Inventory")] Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                db.Entry(campaign).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(campaign);
        }

        // GET: Campaigns/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Campaign campaign = db.Campaigns.Find(id);
            if (campaign == null)
            {
                return HttpNotFound();
            }
            return View(campaign);
        }

        // POST: Campaigns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Campaign campaign = db.Campaigns.Find(id);
            db.Campaigns.Remove(campaign);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [AuthorizeUser(Role = Role.ADMIN)]
        public ActionResult excelCustomer()
        {
            return View();
        }

        [AuthorizeUser(Role = Role.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void excelCustomer([Bind(Include = "StartDate, EndDate")] Campaign cf)
        {
            Debug.WriteLine(cf.StartDate);
            Debug.WriteLine(cf.EndDate);

            var Customers = db.Customers.Include(r => r.Campaign).Include(r => r.Employee).Where(x => x.DateTime >= cf.StartDate).Where(x => x.DateTime <= cf.EndDate).ToList();

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Customer");
            Sheet.Cells["A1"].Value = "ID";
            Sheet.Cells["B1"].Value = "Campaign Name";
            Sheet.Cells["C1"].Value = "Employee Name";
            Sheet.Cells["D1"].Value = "Date Time";
            Sheet.Cells["E1"].Value = "Name";
            Sheet.Cells["F1"].Value = "E-mail";
            Sheet.Cells["G1"].Value = "Phone";
            Sheet.Cells["H1"].Value = "City";
            Sheet.Cells["I1"].Value = "Age";
            Sheet.Cells["J1"].Value = "Gender";
            Sheet.Cells["K1"].Value = "PTCheck";

            int row = 2;
            foreach (var item in Customers)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.CustID;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Campaign.CamName;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Employee.EmpName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.DateTime.ToString("MM/dd/yyyy hh:mm tt");
                Sheet.Cells[string.Format("E{0}", row)].Value = item.CustName;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Email;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Phone;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.City;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.Age;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Gender;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.PTCheck;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
