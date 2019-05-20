using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public class NeedPT : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var customer = (Customer)validationContext.ObjectInstance;
            if (customer.PTCheck == false)
            {

             return new ValidationResult("You must agree to Privacy Policy, Terms & Conditions");

            }
            return ValidationResult.Success;
        }
    }
}