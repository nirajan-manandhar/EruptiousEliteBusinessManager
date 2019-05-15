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
        public string EmpName { get; set; }
        public Role Role { get; set; }
        public EmpStatus EmpStatus { get; set; }
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