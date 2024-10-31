using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Models
{
    public class Admin : AppUser
    {
        public Gender Gender { get; set; } = Gender.Male;
        public int? CreatedById { get; set; }
        public Admin? CreatedBy { get; set; }
        public List<Admin> CreatedAdmins { get; set; } = [];
        public List<Staff> CreatedStaffs { get; set; } = [];
    }
}
