using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool IsActive { get; set; } = true;
        public Customer? Customer { get; set; }
        public Admin? Admin { get; set; }
    }
}
