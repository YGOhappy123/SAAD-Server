using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public Gender Gender { get; set; } = Gender.Male;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? AccountId { get; set; }
        public Account? Account { get; set; }
        public int? CreatedById { get; set; }
        public Admin? CreatedBy { get; set; }
        public List<Admin> CreatedAdmins { get; set; } = [];
        public List<Staff> CreatedStaffs { get; set; } = [];
    }
}
