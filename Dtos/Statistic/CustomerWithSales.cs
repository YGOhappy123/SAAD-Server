using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Auth;

namespace milktea_server.Dtos.Statistic
{
    public class CustomerWithSales
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int? TotalOrdersThisMonth { get; set; } = 0;
        public int? TotalOrdersThisYear { get; set; } = 0;
        public bool? IsActive { get; set; } = true;
    }
}
