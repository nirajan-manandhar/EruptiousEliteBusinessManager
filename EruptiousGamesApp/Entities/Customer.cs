﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public enum Gender
    {
        MALE, FEMALE, OTHER
    }

    public class Customer
    {
        [Key]
        public int CustID { get; set; }
        [Required]
        public int CamID { get; set; }
        [Required]
        public int EmpID { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        public string CustName { get; set; }
        [Required]
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public int? Age { get; set; }
        public Gender? Gender { get; set; }
        [Required]
        public bool PTCheck { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Employee Employee { get; set; }

    }
}