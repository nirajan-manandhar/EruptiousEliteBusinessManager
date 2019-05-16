namespace EruptiousGamesApp.Migrations
{
    using EruptiousGamesApp.Entities;
    using EruptiousGamesApp.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<EruptiousGamesApp.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EruptiousGamesApp.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var terence = new ApplicationUser { UserName = "terence", Employee = new Employee { EmpName = "Terence Barrington", Role = Role.ADMIN, EmpStatus = EmpStatus.ACTIVE, DecksOnHand = 0 } };
            UserManager.Create(terence, "terence4900");

            Dummy(context);
        }

        private void Dummy(ApplicationDbContext context)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var Admin = new ApplicationUser { UserName = "admin", Employee = new Employee { EmpName = "Admin", Role = Role.ADMIN, EmpStatus = EmpStatus.ACTIVE, DecksOnHand = 0 } };
            UserManager.Create(Admin, "123456");
            var Manager = new ApplicationUser { UserName = "manager", Employee = new Employee { EmpName = "Manager", Role = Role.MANAGER, EmpStatus = EmpStatus.ACTIVE, DecksOnHand = 0 } };
            UserManager.Create(Manager, "123456");
            var Hank = new ApplicationUser { UserName = "hank", Employee = new Employee { EmpName = "Hank", Role = Role.AMBASSADOR, EmpStatus = EmpStatus.ACTIVE, DecksOnHand = 0 } };
            UserManager.Create(Hank, "123456");
            var Aska = new ApplicationUser { UserName = "aska", Employee = new Employee { EmpName = "Aska", Role = Role.AMBASSADOR, EmpStatus = EmpStatus.ACTIVE, DecksOnHand = 0 } };
            UserManager.Create(Aska, "123456");

            context.Configuration.ValidateOnSaveEnabled = false;

            var campaigns = new List<Campaign>
            {
                new Campaign { CamName = "Campaign1", StartDate = DateTime.Parse("2019-05-01"), EndDate=DateTime.Parse("2019-05-31"), Inventory=100 }
            };
            campaigns.ForEach(c => context.Campaigns.AddOrUpdate(p => p.CamName, c));
            context.SaveChanges();

            context.Configuration.ValidateOnSaveEnabled = true;

            context = new ApplicationDbContext();
            AssignEmployeeToCampaign(context, 4, 1);
            context.SaveChanges();

        }

        private void AssignEmployeeToCampaign(ApplicationDbContext context, int EmpID, int CamID)
        {
            var employee = context.Employees.SingleOrDefault(e => e.EmpID == EmpID);
            Campaign campaign = null;
            try
            {
                campaign = employee.Campaigns.SingleOrDefault(c => c.CamID == CamID);
            }
            catch (ArgumentNullException)
            {
            }

            if (campaign == null)
                employee.Campaigns.Add(context.Campaigns.Single(c => c.CamID == CamID));
        }

    }
}
