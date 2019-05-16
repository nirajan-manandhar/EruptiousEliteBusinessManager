using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public class Campaign
    {
        [Key]
        public int CamID { get; set; }
        [Required]
        public string CamName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [StartEndDateValidation]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        [RegularExpression("[0-9]{1,}", ErrorMessage = "Please enter 0 or an positive integer")]
        public int Inventory { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Work> Works { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}