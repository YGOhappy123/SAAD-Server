using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Dtos.Auth
{
    public class AdminDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Gender { get; set; } = UserGender.Male.ToString();
        public string Role { get; } = UserRole.Admin.ToString();
    }
}