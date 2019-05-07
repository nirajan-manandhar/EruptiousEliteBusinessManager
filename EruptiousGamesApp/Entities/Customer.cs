using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public enum Gender
    {
        MALE, FEMALE
    }

    public class Customer
    {
        [Key]
        public int CustID { get; set; }
        public int CamID { get; set; }
        public int EmpID { get; set; }
        public DateTime DateTime { get; set; }
        public string CustName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public bool PTCheck { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Employee Employee { get; set; }

    }
}