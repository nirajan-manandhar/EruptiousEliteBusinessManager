using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public class StartEndDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var campaign = (Campaign)validationContext.ObjectInstance;
            if (DateTime.Compare(campaign.StartDate, campaign.EndDate) >= 0 )
            {
                return new ValidationResult("Start Date must be before End Date");
            }
            return ValidationResult.Success;
        }
    }
}