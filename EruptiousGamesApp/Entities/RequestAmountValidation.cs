using EruptiousGamesApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace EruptiousGamesApp.Entities
{
    public class RequestAmountValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var request = (Request)validationContext.ObjectInstance;

            ApplicationDbContext db = new ApplicationDbContext();
            string currentUserId = HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser currentUser = (db.Users.Include(r => r.Employee).Include(r => r.Employee.Campaigns).FirstOrDefault(x => x.Id == currentUserId));

            if ((int)request.Action == 0)
            {
                if (request.Amount > currentUser.GetTodaysCampaign().Inventory)
                {
                    return new ValidationResult("The amount requested exceeds the number of inventory available.");
                }
            } else
            {
                if (request.Amount > currentUser.Employee.DecksOnHand)
                {
                    return new ValidationResult("You are attempting to return more decks than you have on hand.");
                }
            }
            return ValidationResult.Success;
        }
    }
}