using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<Admin?> GetAdminById(int adminId);
        Task<Admin?> GetAdminByAccountId(int accountId);
        Task<(List<Admin>, int)> GetAllAdmins(BaseQueryObject queryObject);
        Task AddAdmin(Admin admin);
        Task UpdateAdmin(Admin admin);
    }
}
