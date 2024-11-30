using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Auth;
using milktea_server.Models;

namespace milktea_server.Extensions.Mappers
{
    public static class AdminMapper
    {
        public static AdminDto ToAdminDto(this Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
                Avatar = admin.Avatar,
                CreatedAt = admin.CreatedAt,
                CreatedById = admin.CreatedBy?.Id,
                CreatedBy = admin.CreatedBy == null ? null : $"{admin.CreatedBy.LastName} {admin.CreatedBy.FirstName}",
                IsActive = admin.Account?.IsActive,
            };
        }
    }
}
