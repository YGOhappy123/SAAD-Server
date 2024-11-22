using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Response;
using milktea_server.Interfaces.Repositories;
using milktea_server.Interfaces.Services;
using milktea_server.Models;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Services
{
    public class ProductService : IProductService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMilkteaRepository _milkteaRepo;
        private readonly IToppingRepository _toppingRepo;

        public ProductService(ICategoryRepository CategoryRepo, IMilkteaRepository milkteaRepo, IToppingRepository toppingRepo)
        {
            _categoryRepo = CategoryRepo;
            _milkteaRepo = milkteaRepo;
            _toppingRepo = toppingRepo;
        }

        public async Task<ServiceResponse<List<Category>>> GetAllCategories(BaseQueryObject queryObject)
        {
            var (categories, total) = await _categoryRepo.GetAllCategories(queryObject);

            return new ServiceResponse<List<Category>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = categories,
                Total = total,
                Took = categories.Count,
            };
        }

        public async Task<ServiceResponse<List<Milktea>>> GetAllMilkteas(BaseQueryObject queryObject)
        {
            var (milkteas, total) = await _milkteaRepo.GetAllMilkteas(queryObject);

            return new ServiceResponse<List<Milktea>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = milkteas,
                Total = total,
                Took = milkteas.Count,
            };
        }

        public async Task<ServiceResponse<List<Topping>>> GetAllToppings(BaseQueryObject queryObject)
        {
            var (toppings, total) = await _toppingRepo.GetAllToppings(queryObject);

            return new ServiceResponse<List<Topping>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = toppings,
                Total = total,
                Took = toppings.Count,
            };
        }
    }
}
