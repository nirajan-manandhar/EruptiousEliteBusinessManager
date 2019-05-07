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
        public string CamName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Inventory { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Work> Works { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Request> Requests { get; set; }
    }
}