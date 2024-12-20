using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using milktea_server.Data;
using milktea_server.Enums;
using milktea_server.Interfaces.Repositories;
using milktea_server.Models;

namespace milktea_server.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public AccountRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<Account?> GetAccountByUsername(string username)
        {
            return await _dbContext.Accounts.SingleOrDefaultAsync(acc => acc.IsActive && acc.Username == username);
        }

        public async Task<Account?> GetAccountById(int accountId)
        {
            return await _dbContext.Accounts.SingleOrDefaultAsync(acc => acc.IsActive && acc.Id == accountId);
        }

        public async Task<Account?> GetCustomerAccountByEmail(string email)
        {
            return await _dbContext
                .Accounts.Where(acc => acc.IsActive && acc.Customer != null && acc.Customer.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<Account?> GetAccountByUserIdAndRole(int userId, string role)
        {
            if (role == UserRole.Customer.ToString())
            {
                return await _dbContext
                    .Accounts.Where(acc => acc.IsActive && acc.Customer != null && acc.Customer.Id == userId)
                    .FirstOrDefaultAsync();
            }
            else
            {
                return await _dbContext
                    .Accounts.Where(acc => acc.IsActive && acc.Admin != null && acc.Admin.Id == userId)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task AddAccount(Account account)
        {
            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAccount(Account account)
        {
            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
        }
    }
}
