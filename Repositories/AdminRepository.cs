using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using milktea_server.Data;
using milktea_server.Interfaces.Repositories;
using milktea_server.Models;

namespace milktea_server.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public AdminRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<Admin?> GetAdminById(int adminId)
        {
            return await _dbContext.Admins.SingleOrDefaultAsync(ad => ad.Id == adminId);
        }

        public async Task<Admin?> GetAdminByAccountId(int accountId)
        {
            return await _dbContext.Admins.SingleOrDefaultAsync(ad => ad.AccountId == accountId);
        }

        public async Task AddAdmin(Admin admin)
        {
            _dbContext.Admins.Add(admin);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAdmin(Admin admin)
        {
            _dbContext.Admins.Update(admin);
            await _dbContext.SaveChangesAsync();
        }
    }
}
