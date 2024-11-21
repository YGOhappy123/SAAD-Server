using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Models;

namespace milktea_server.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin?> GetAdminById(int adminId);
        Task<Admin?> GetAdminByAccountId(int accountId);
        Task AddAdmin(Admin admin);
        Task UpdateAdmin(Admin admin);
    }
}
