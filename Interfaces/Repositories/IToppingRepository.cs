using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Repositories
{
    public interface IToppingRepository
    {
        Task<(List<Topping>, int)> GetAllToppings(BaseQueryObject queryObject);
    }
}
