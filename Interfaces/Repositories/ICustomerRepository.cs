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
        Task<Customer?> GetCustomerByEmail(string email);
        Task<(List<Customer>, int)> GetAllCustomers(BaseQueryObject queryObject);
        Task AddCustomer(Customer customer);
        Task UpdateCustomer(Customer customer);
        Task<int> CountCustomersCreatedInTimeRange(DateTime startTime, DateTime endTime);
        Task<List<Customer>> GetNewestCustomers(DateTime startTime, DateTime endTime, int limit);
        Task<List<Customer>> GetCustomersWithHighestTotalOrderValue(DateTime startTime, DateTime endTime, int limit);
    }
}
