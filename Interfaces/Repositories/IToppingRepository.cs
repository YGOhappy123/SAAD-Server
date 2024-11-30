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
        Task<Topping?> GetToppingById(int toppingId);
        Task<List<Topping>> GetToppingsByName(string nameVi, string nameEn);
        Task AddTopping(Topping topping);
        Task UpdateTopping(Topping topping);
    }
}
