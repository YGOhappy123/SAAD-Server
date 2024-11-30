using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using milktea_server.Dtos.Product;
using milktea_server.Dtos.Response;
using milktea_server.Dtos.Statistic;
using milktea_server.Extensions.Mappers;
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
        private readonly ICartRepository _cartRepo;
        private readonly IOrderRepository _orderRepo;

        public ProductService(
            ICategoryRepository CategoryRepo,
            IMilkteaRepository milkteaRepo,
            IToppingRepository toppingRepo,
            ICartRepository cartRepo,
            IOrderRepository orderRepo
        )
        {
            _categoryRepo = CategoryRepo;
            _milkteaRepo = milkteaRepo;
            _toppingRepo = toppingRepo;
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
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

        public async Task<ServiceResponse> CreateNewCategory(CreateUpdateCategoryDto createCategoryDto)
        {
            var categoryWithSameName = await _categoryRepo.GetCategoryByName(createCategoryDto.NameVi, createCategoryDto.NameEn);
            if (categoryWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.DUPLICATE_CATEGORY_NAME,
                };
            }

            var newCategory = new Category
            {
                NameVi = createCategoryDto.NameVi.CapitalizeEachWords(),
                NameEn = createCategoryDto.NameEn.CapitalizeEachWords(),
            };
            await _categoryRepo.AddCategory(newCategory);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_CATEGORY_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateCategory(CreateUpdateCategoryDto updateCategoryDto, int categoryId)
        {
            var targetCategory = await _categoryRepo.GetCategoryById(categoryId);
            if (targetCategory == null || !targetCategory.IsActive)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CATEGORY_NOT_FOUND_OR_UNAVAILABLE,
                };
            }

            var categoryWithSameName = await _categoryRepo.GetCategoryByName(updateCategoryDto.NameVi, updateCategoryDto.NameEn);
            if (categoryWithSameName != null && categoryWithSameName.Id != categoryId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.DUPLICATE_CATEGORY_NAME,
                };
            }

            targetCategory.NameVi = updateCategoryDto.NameVi.CapitalizeEachWords();
            targetCategory.NameEn = updateCategoryDto.NameEn.CapitalizeEachWords();
            await _categoryRepo.UpdateCategory(targetCategory);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_CATEGORY_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> ToggleCategoryActiveStatus(int categoryId)
        {
            var targetCategory = await _categoryRepo.GetCategoryById(categoryId);
            if (targetCategory == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CATEGORY_NOT_FOUND,
                };
            }

            bool newStatus = !targetCategory.IsActive;
            targetCategory.IsActive = newStatus;

            await _categoryRepo.UpdateCategory(targetCategory);
            if (newStatus == true)
            {
                await _milkteaRepo.DisableMilkteasRelatedToCategory(categoryId);
                await _cartRepo.RemoveCartItemsRelatedToCategory(categoryId);
            }

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = newStatus ? SuccessMessage.ENABLE_CATEGORY_SUCCESSFULLY : SuccessMessage.DISABLE_CATEGORY_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<List<MilkteaWithSales>>> SearchMilkteas(string searchTerm)
        {
            var milkteas = await _milkteaRepo.SearchMilkteasByName(searchTerm);

            List<MilkteaWithSales> milkteasWithSales = [];
            foreach (var milktea in milkteas)
            {
                var soldUnitsThisMonth = await _orderRepo.CountMilkteaSoldUnits(milktea.Id, "monthly");

                milkteasWithSales.Add(
                    new MilkteaWithSales
                    {
                        Id = milktea.Id,
                        NameVi = milktea.NameVi,
                        NameEn = milktea.NameEn,
                        Image = milktea.Image,
                        Price = new MilkteaPrice
                        {
                            S = milktea.PriceS,
                            M = milktea.PriceM,
                            L = milktea.PriceL,
                        },
                        SoldUnitsThisMonth = soldUnitsThisMonth,
                    }
                );
            }

            return new ServiceResponse<List<MilkteaWithSales>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = milkteasWithSales,
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

        public async Task<ServiceResponse<object>> GetAllMilkteaById(int milkteaId)
        {
            var milktea = await _milkteaRepo.GetMilkteaById(milkteaId);
            if (milktea == null || !milktea.IsActive)
            {
                return new ServiceResponse<object>
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_NOT_FOUND,
                };
            }

            var currentTime = TimestampHandler.GetNow();
            var startOfCurrentTime = TimestampHandler.GetStartOfTimeByType(currentTime, "yearly");
            var statisticThisYear = await _milkteaRepo.GetMilkteaStatisticInTimeRange(startOfCurrentTime, currentTime, milkteaId);

            var milkteaDto = milktea.ToMilkteaDto();
            milkteaDto.SoldUnitsThisYear = statisticThisYear.Item1;

            return new ServiceResponse<object>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = milkteaDto,
            };
        }

        public async Task<ServiceResponse> CreateNewMilktea(CreateUpdateMilkteaDto createMilkteaDto)
        {
            var milkteaWithSameName = await _milkteaRepo.GetMilkteaByName(createMilkteaDto.NameVi, createMilkteaDto.NameEn);
            if (milkteaWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.DUPLICATE_PRODUCT_NAME,
                };
            }

            var newMilktea = new Milktea
            {
                NameVi = createMilkteaDto.NameVi.CapitalizeEachWords(),
                NameEn = createMilkteaDto.NameEn.CapitalizeEachWords(),
                DescriptionVi = createMilkteaDto.DescriptionVi,
                DescriptionEn = createMilkteaDto.DescriptionEn,
                CategoryId = createMilkteaDto.CategoryId,
                PriceS = createMilkteaDto?.PriceS,
                PriceM = createMilkteaDto?.PriceM,
                PriceL = createMilkteaDto?.PriceL,
                Image = createMilkteaDto?.Image,
            };
            await _milkteaRepo.AddMilktea(newMilktea);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_PRODUCT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateMilktea(CreateUpdateMilkteaDto updateMilkteaDto, int milkteaId)
        {
            var targetMilktea = await _milkteaRepo.GetMilkteaById(milkteaId);
            if (targetMilktea == null || !targetMilktea.IsActive)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_NOT_FOUND_OR_UNAVAILABLE,
                };
            }

            var milkteaWithSameName = await _milkteaRepo.GetMilkteaByName(updateMilkteaDto.NameVi, updateMilkteaDto.NameEn);
            if (milkteaWithSameName != null && milkteaWithSameName.Id != milkteaId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.DUPLICATE_PRODUCT_NAME,
                };
            }

            targetMilktea.NameVi = updateMilkteaDto.NameVi.CapitalizeEachWords();
            targetMilktea.NameEn = updateMilkteaDto.NameEn.CapitalizeEachWords();
            targetMilktea.DescriptionVi = updateMilkteaDto.DescriptionVi;
            targetMilktea.DescriptionEn = updateMilkteaDto.DescriptionEn;
            targetMilktea.CategoryId = updateMilkteaDto.CategoryId;
            targetMilktea.PriceS = updateMilkteaDto?.PriceS;
            targetMilktea.PriceM = updateMilkteaDto?.PriceM;
            targetMilktea.PriceL = updateMilkteaDto?.PriceL;
            targetMilktea.Image = updateMilkteaDto?.Image;
            targetMilktea.IsAvailable = updateMilkteaDto?.IsAvailable ?? true;
            await _milkteaRepo.UpdateMilktea(targetMilktea);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_PRODUCT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> ToggleMilkteaActiveStatus(int milkteaId)
        {
            var targetMilktea = await _milkteaRepo.GetMilkteaById(milkteaId);
            if (targetMilktea == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_NOT_FOUND,
                };
            }

            bool newStatus = !targetMilktea.IsActive;
            targetMilktea.IsActive = newStatus;
            targetMilktea.IsAvailable = false;

            await _milkteaRepo.UpdateMilktea(targetMilktea);
            if (newStatus == true)
            {
                await _cartRepo.RemoveCartItemsRelatedToMilktea(milkteaId);
            }

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = newStatus ? SuccessMessage.ENABLE_PRODUCT_SUCCESSFULLY : SuccessMessage.DISABLE_PRODUCT_SUCCESSFULLY,
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

        public async Task<ServiceResponse> CreateNewTopping(CreateUpdateToppingDto createToppingDto)
        {
            var toppingWithSameName = await _toppingRepo.GetToppingByName(createToppingDto.NameVi, createToppingDto.NameEn);
            if (toppingWithSameName != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.DUPLICATE_PRODUCT_NAME,
                };
            }

            var newTopping = new Topping
            {
                NameVi = createToppingDto.NameVi.CapitalizeEachWords(),
                NameEn = createToppingDto.NameEn.CapitalizeEachWords(),
                DescriptionVi = createToppingDto.DescriptionVi,
                DescriptionEn = createToppingDto.DescriptionEn,
                Price = createToppingDto.Price,
                Image = createToppingDto?.Image,
            };
            await _toppingRepo.AddTopping(newTopping);

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_PRODUCT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateTopping(CreateUpdateToppingDto updateToppingDto, int toppingId)
        {
            var targetTopping = await _toppingRepo.GetToppingById(toppingId);
            if (targetTopping == null || !targetTopping.IsActive)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_NOT_FOUND_OR_UNAVAILABLE,
                };
            }

            var toppingWithSameName = await _toppingRepo.GetToppingByName(updateToppingDto.NameVi, updateToppingDto.NameEn);
            if (toppingWithSameName != null && toppingWithSameName.Id != toppingId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.DUPLICATE_PRODUCT_NAME,
                };
            }

            targetTopping.NameVi = updateToppingDto.NameVi.CapitalizeEachWords();
            targetTopping.NameEn = updateToppingDto.NameEn.CapitalizeEachWords();
            targetTopping.DescriptionVi = updateToppingDto.DescriptionVi;
            targetTopping.DescriptionEn = updateToppingDto.DescriptionEn;
            targetTopping.Price = updateToppingDto.Price;
            targetTopping.Image = updateToppingDto?.Image;
            targetTopping.IsAvailable = updateToppingDto?.IsAvailable ?? true;
            await _toppingRepo.UpdateTopping(targetTopping);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_PRODUCT_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> ToggleToppingActiveStatus(int toppingId)
        {
            var targetTopping = await _toppingRepo.GetToppingById(toppingId);
            if (targetTopping == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.PRODUCT_NOT_FOUND,
                };
            }

            bool newStatus = !targetTopping.IsActive;
            targetTopping.IsActive = newStatus;
            targetTopping.IsAvailable = false;

            await _toppingRepo.UpdateTopping(targetTopping);
            if (newStatus == true)
            {
                await _cartRepo.RemoveCartItemsRelatedToTopping(toppingId);
            }

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = newStatus ? SuccessMessage.ENABLE_PRODUCT_SUCCESSFULLY : SuccessMessage.DISABLE_PRODUCT_SUCCESSFULLY,
            };
        }
    }
}
