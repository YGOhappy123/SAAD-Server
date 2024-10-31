using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Models
{
    public class Customer : AppUser
    {
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
