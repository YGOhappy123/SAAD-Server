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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CustomerRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<Customer?> GetCustomerById(int customerId)
        {
            return await _dbContext.Customers.SingleOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<Customer?> GetCustomerByAccountId(int accountId)
        {
            return await _dbContext.Customers.SingleOrDefaultAsync(c => c.AccountId == accountId);
        }

        public async Task<Customer?> GetCustomerByEmail(string email, bool isAccountIncluded)
        {
            if (isAccountIncluded)
            {
                return await _dbContext.Customers.Include(c => c.Account).SingleOrDefaultAsync(c => c.Email == email);
            }
            else
            {
                return await _dbContext.Customers.SingleOrDefaultAsync(c => c.Email == email);
            }
        }

        public async Task AddCustomer(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCustomer(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            await _dbContext.SaveChangesAsync();
        }
    }
}
