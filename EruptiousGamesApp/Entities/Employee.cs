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
        public string UserName { get; set; }
        public string Password { get; set; }
        public EmpStatus EmpStatus { get; set; }

    }
}