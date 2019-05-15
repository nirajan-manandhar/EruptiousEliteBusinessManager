using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public class Note
    {
        public Note()
        {
            DateTime = DateTime.Now;
        }
        [Key]
        public int NoteID { get; set; }
        [Required]
        public int EmpID { get; set; }
        [Required]
        public DateTime DateTime { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }

        public virtual Employee Employee { get; set; }
    }
}