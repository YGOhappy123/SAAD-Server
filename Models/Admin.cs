using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Models
{
    public class Admin : AppUser
    {
        public int? CreatedById { get; set; }
        public Admin? CreatedBy { get; set; }
        public List<Admin> CreatedAdmins { get; set; } = [];
    }
}
