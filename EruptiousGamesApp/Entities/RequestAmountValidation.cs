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

            if ((int)request.Action == 0)
            {
                if (request.Amount > db.Campaigns.Find(request.CamID).Inventory)
                {
                    return new ValidationResult("The amount requested exceeds the number of inventory available.");
                }
            } else
            {
                if (request.Amount > db.Employees.Find(request.EmpID).DecksOnHand)
                {
                    return new ValidationResult("This employee is attempting to return more decks than the employee has on hand.");
                }
            }
            return ValidationResult.Success;
        }
    }
}