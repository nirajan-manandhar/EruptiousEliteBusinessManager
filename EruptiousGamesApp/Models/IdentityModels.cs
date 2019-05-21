using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using EruptiousGamesApp.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EruptiousGamesApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public Employee Employee { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public Campaign GetTodaysCampaign()
        {
            foreach (Campaign c in Employee.Campaigns)
            {
                if (c.StartDate <= DateTime.Today && c.EndDate >= DateTime.Today)
                {
                    return c;
                }
            }

            return null;
        }

    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<EruptiousGamesApp.Entities.Campaign> Campaigns { get; set; }

        public System.Data.Entity.DbSet<EruptiousGamesApp.Entities.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<EruptiousGamesApp.Entities.Note> Notes { get; set; }

        public System.Data.Entity.DbSet<EruptiousGamesApp.Entities.Request> Requests { get; set; }

        public System.Data.Entity.DbSet<EruptiousGamesApp.Entities.Work> Works { get; set; }
    }
}