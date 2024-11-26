using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerById(int customerId);
        Task<Customer?> GetCustomerByAccountId(int accountId);
        Task<Customer?> GetCustomerByEmail(string email, bool isAccountIncluded = false);
        Task<(List<Customer>, int)> GetAllCustomers(BaseQueryObject queryObject);
        Task AddCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
    }
}
