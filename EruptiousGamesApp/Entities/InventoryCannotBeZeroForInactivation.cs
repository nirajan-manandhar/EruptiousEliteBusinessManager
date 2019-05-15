using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public class InventoryCannotBeZeroForInactivation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var employee = (Employee)validationContext.ObjectInstance;
            if ((int)employee.EmpStatus == 1)
            {
                if (employee.DecksOnHand > 0)
                {
                    return new ValidationResult("This employee still has inventory. Cannot inactivate until inventory is returned.");
                }
            }
            return ValidationResult.Success;
        }
    }
}