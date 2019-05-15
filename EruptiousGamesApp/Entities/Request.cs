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
        PENDING, APPROVED, DENIAL
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
        [Range(1, 20)]
        [RegularExpression("([0-9])", ErrorMessage = "Please enter an integer between 1 and 20")]
        public int Amount { get; set; }
        [Required]
        public Action Action { get; set; }
        [Required]
        public RequestStatus RequestStatus { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Employee Employee { get; set; }
    }
}