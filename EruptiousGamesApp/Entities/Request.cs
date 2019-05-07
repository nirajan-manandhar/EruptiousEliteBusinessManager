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
        [Key]
        public int RequestID { get; set; }
        public int CamID { get; set; }
        public int EmpID { get; set; }
        public DateTime DateTime { get; set; }
        public int Amount { get; set; }
        public Action Action { get; set; }
        public RequestStatus RequestStatus { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Employee Employee { get; set; }
    }
}