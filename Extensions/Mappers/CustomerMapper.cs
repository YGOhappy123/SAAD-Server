using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Auth;
using milktea_server.Models;

namespace milktea_server.Extensions.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToCustomerDto(this Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Avatar = customer.Avatar,
                CreatedAt = customer.CreatedAt,
                PhoneNumber = customer.PhoneNumber,
                Address = customer.Address,
            };
        }
    }
}
