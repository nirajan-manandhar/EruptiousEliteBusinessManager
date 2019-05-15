using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public class Work
    {
        public Work(){
            Date = DateTime.Today;
        }

        [Key]
        public int WorkID { get; set; }
        public int CamID { get; set; }
        public int EmpID { get; set; }
        public DateTime Date { get; set;}
        public int Sold { get; set; }
        public int CustomerPlayWith { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Employee Employee { get; set; }
    }
}