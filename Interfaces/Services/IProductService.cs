using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Response;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Services
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Category>>> GetAllCategories(BaseQueryObject queryObject);
        Task<ServiceResponse<List<Milktea>>> GetAllMilkteas(BaseQueryObject queryObject);
        Task<ServiceResponse<List<Topping>>> GetAllToppings(BaseQueryObject queryObject);
    }
}
