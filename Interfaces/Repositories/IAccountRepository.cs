using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Models;

namespace milktea_server.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByUsername(string username);
        Task<Account?> GetAccountById(int accountId);
        Task<Account?> GetCustomerAccountByEmail(string email);
        Task<Account?> GetAccountByUserIdAndRole(int userId, string role);
        Task AddAccount(Account account);
        Task UpdateAccount(Account account);
    }
}
