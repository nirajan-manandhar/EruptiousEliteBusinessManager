using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public enum Action
    {
        REQUEST, RETURN
    }
    public enum RequestStatus
    {
        PENDING, APPROVED, REJECTED
    }


    public class Request
    {

        public Request()
        {
            DateTime = DateTime.Now;
        }

        [Key]
        public int RequestID { get; set; }
        [Required]
        public int CamID { get; set; }
        [Required]
        public int EmpID { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        [Range(1, 100)]
        [RegularExpression("[0-9]{1,}", ErrorMessage = "Please enter an integer between 1 and 100")]
        public int Amount { get; set; }
        [Required]
        [RequestAmountValidation]
        public Action Action { get; set; }
        [Required]
        public RequestStatus RequestStatus { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Employee Employee { get; set; }
    }
}