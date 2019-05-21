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
        public class CampaignEmployee
        {
            public Campaign campaign;
            public IEnumerable<Employee> assignedEmployeeList;
            public IEnumerable<Employee> unassignedEmployeeList;
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Campaigns/ (Default) OR Campaigns/Index 
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult Index()
        {
            return View(db.Campaigns.ToList());
        }

        // GET: Campaigns/Create
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult Create()
        {
            Campaign campaign = new Campaign
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(9)
            };
            return View("Create", campaign);
        }

        // POST: Campaigns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult Create([Bind(Include = "CamID,CamName,StartDate,EndDate,Inventory")] Campaign campaign)
        {

            if (ModelState.IsValid)
            {
                db.Campaigns.Add(campaign);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campaign);
        }

        //GET: Campaigns/AssignEmp
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult AssignEmp(int? id)//This id is CamID
        {
       
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Campaign campaign = db.Campaigns.Include(c => c.Employees).FirstOrDefault(c => c.CamID == id);

            if (campaign == null)
            {
                return HttpNotFound();
            }


            var assignedEmployeeList = campaign.Employees.ToList(); //this is the list of the employees assigned to this Campaign
            var unassignedEmployeeList = db.Employees.ToList();  //This is the list of all the employees in the company

            foreach (Employee e in assignedEmployeeList.ToList())
            {
                unassignedEmployeeList.Remove(e);
            }

            CampaignEmployee ce = new CampaignEmployee
            {
                campaign = campaign,
                assignedEmployeeList = assignedEmployeeList.OrderBy(r => r.EmpName),
                unassignedEmployeeList = unassignedEmployeeList.OrderBy(r => r.EmpName)
            };
            return View(ce);

        }

        //Action Method to assign an Employee to a Campaign
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult AssignAction(int CamId, int EmpId)//Assign this EmpId to this CamId
        {
            Campaign campaign = db.Campaigns.Include(c => c.Employees).FirstOrDefault(c => c.CamID == CamId);
            Employee employee = db.Employees.Include(e => e.Campaigns).FirstOrDefault(e => e.EmpID == EmpId);

            if(!(campaign.EndDate < DateTime.Today)) {
                foreach (Campaign c in employee.Campaigns)
                {
                    if (!(c.EndDate < DateTime.Today)) {
                        if (!(DateTime.Compare(c.StartDate, campaign.EndDate) > 0) && !(DateTime.Compare(c.EndDate, campaign.StartDate) < 0))
                        {
                            TempData["error"] = "There is overlap in date between this campaign and the employees' campaigns";
                            return RedirectToAction("AssignEmp", new { id = CamId });
                        }
                    }
                }
            }

            Campaign employeeCampaign = employee.GetTodaysCampaign();

            campaign.Employees.Add(employee);

            db.SaveChanges();

            return RedirectToAction("AssignEmp", new { id = CamId });
        }

        //Action method to remove an Employee that was assigned to a Campaign
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult RemoveAction(int CamId, int EmpId)//Remove this EmpId to this CamId
        {
            Campaign campaign = db.Campaigns.Include(c => c.Employees).FirstOrDefault(c => c.CamID == CamId);
            Employee employee = db.Employees.Find(EmpId);

            campaign.Employees.Remove(employee);

            db.SaveChanges();

            return RedirectToAction("AssignEmp", new { id = CamId });
        }

        [AuthorizeUser(Role = Role.MANAGER)]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeUser(Role = Role.MANAGER)]
        public ActionResult Edit([Bind(Include = "CamID,CamName,StartDate,EndDate,Inventory")] Campaign campaign, DateTime previousStartDate, DateTime previousEndDate, int campaignID)
        {
            int i = campaign.CamID;
            if (ModelState.IsValid)
            {
                var cam = db.Campaigns.Find(i);
                if (previousStartDate.Date != campaign.StartDate.Date || previousEndDate.Date != campaign.EndDate.Date)
                {
                    cam.Employees.Clear();
                    cam.StartDate = campaign.StartDate;
                    cam.EndDate = campaign.EndDate;
                }
                cam.CamName = campaign.CamName;
                cam.Inventory = campaign.Inventory;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(campaign);
        }

       //GET: Campaigns/Delete/5
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

        //GET Excel 
        [AuthorizeUser(Role = Role.ADMIN)]
        public ActionResult excelCustomer()
        {
            return View();
        }

        //POST Excel 
        [AuthorizeUser(Role = Role.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void excelCustomer([Bind(Include = "StartDate, EndDate")] Campaign cf)
        {
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
                try
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.CustID;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.Campaign.CamName;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Employee.EmpName;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.DateTime.ToString("MM/dd/yyyy hh:mm tt");
                    Sheet.Cells[string.Format("E{0}", row)].Value = Encryptor.Decrypt(item.CustName);
                    Sheet.Cells[string.Format("F{0}", row)].Value = Encryptor.Decrypt(item.Email);
                    if (item.Phone != null)
                    {
                        Sheet.Cells[string.Format("G{0}", row)].Value = Encryptor.Decrypt(item.Phone);
                    }
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.City;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Age;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.Gender;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.PTCheck;
                    row++;
                }
                catch (System.ArgumentNullException)
                {

                }
                catch (System.Security.Cryptography.CryptographicException)
                {

                }

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
