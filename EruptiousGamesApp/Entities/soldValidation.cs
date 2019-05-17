using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EruptiousGamesApp.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using static EruptiousGamesApp.Controllers.NotesController;

namespace EruptiousGamesApp.Entities
{
    public class SoldValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var work = (Work)validationContext.ObjectInstance;
            ApplicationDbContext db = new ApplicationDbContext();

            string currentUserId = HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            if (work.Sold > currentUser.Employee.DecksOnHand)
            {
                return new ValidationResult("You don't have enough decks on hand.");
            }
            return ValidationResult.Success;
        }
    }
}