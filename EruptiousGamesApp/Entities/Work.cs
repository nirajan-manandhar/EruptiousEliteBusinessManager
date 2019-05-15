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
        [Required]
        public int CamID { get; set; }
        [Required]
        public int EmpID { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set;}
        [Required]
        public int Sold { get; set; }
        [Required]
        public int CustomerPlayWith { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual Employee Employee { get; set; }
    }
}