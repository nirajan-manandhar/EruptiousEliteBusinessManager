using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public class Note
    {
        [Key]
        public int NoteID { get; set; }
        public int EmpID { get; set; }
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public virtual Employee Employee { get; set; }
    }
}