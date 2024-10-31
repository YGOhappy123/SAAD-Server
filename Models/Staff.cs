using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Models
{
    public class Staff : AppUser
    {
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Gender Gender { get; set; } = Gender.Male;
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10,2)")]
        public decimal MonthlySalary { get; set; }
        public int? CreatedById { get; set; }
        public Admin? CreatedBy { get; set; }
    }
}
