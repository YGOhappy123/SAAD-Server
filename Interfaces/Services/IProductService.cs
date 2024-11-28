using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Product;
using milktea_server.Dtos.Response;
using milktea_server.Dtos.Statistic;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Services
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Category>>> GetAllCategories(BaseQueryObject queryObject);
        Task<ServiceResponse> CreateNewCategory(CreateUpdateCategoryDto createCategoryDto);
        Task<ServiceResponse> UpdateCategory(CreateUpdateCategoryDto updateCategoryDto, int categoryId);
        Task<ServiceResponse> ToggleCategoryActiveStatus(int categoryId);

        Task<ServiceResponse<List<MilkteaWithSales>>> SearchMilkteas(string searchTerm);
        Task<ServiceResponse<List<Milktea>>> GetAllMilkteas(BaseQueryObject queryObject);
        Task<ServiceResponse> CreateNewMilktea(CreateUpdateMilkteaDto createMilkteaDto);
        Task<ServiceResponse> UpdateMilktea(CreateUpdateMilkteaDto updateMilkteaDto, int milkteaId);
        Task<ServiceResponse> ToggleMilkteaActiveStatus(int milkteaId);

        Task<ServiceResponse<List<Topping>>> GetAllToppings(BaseQueryObject queryObject);
        Task<ServiceResponse> CreateNewTopping(CreateUpdateToppingDto createToppingDto);
        Task<ServiceResponse> UpdateTopping(CreateUpdateToppingDto updateToppingDto, int toppingId);
        Task<ServiceResponse> ToggleToppingActiveStatus(int toppingId);
    }
}
