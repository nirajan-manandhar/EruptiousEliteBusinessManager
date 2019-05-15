using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EruptiousGamesApp.Entities
{
    public enum Role
    {
        AMBASSADOR, MANAGER, ADMIN
    }

    public enum EmpStatus
    {
        ACTIVE, INACTIVE
    }

    public class Employee
    {
        [Key]
        public int EmpID { get; set; }
        [Required]
        public string EmpName { get; set; }
        [Required]
        public Role Role { get; set; }
        [Required]
        public EmpStatus EmpStatus { get; set; }
        [Required]
        public int DecksOnHand { get; set; }


        public virtual ICollection<Campaign> Campaigns { get; set; }
        public virtual ICollection<Work> Workings { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Request> Requests { get; set; }

        public Campaign GetTodaysCampaign()
        {

            return null;
        }
    }
}