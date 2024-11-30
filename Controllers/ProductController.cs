using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using milktea_server.Dtos.Product;
using milktea_server.Dtos.Response;
using milktea_server.Extensions.Mappers;
using milktea_server.Interfaces.Services;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Controllers
{
    [ApiController]
    [Route("/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _productService.GetAllCategories(queryObject);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Data = result.Data,
                    Took = result.Took,
                    Total = result.Total,
                }
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        public async Task<IActionResult> CreateNewCategory([FromBody] CreateUpdateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _productService.CreateNewCategory(createCategoryDto);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("categories/{categoryId:int}")]
        public async Task<IActionResult> UpdateCategory([FromBody] CreateUpdateCategoryDto updateCategoryDto, [FromRoute] int categoryId)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _productService.UpdateCategory(updateCategoryDto, categoryId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("categories/toggle-active/{categoryId:int}")]
        public async Task<IActionResult> ToggleCategoryActiveStatus([FromRoute] int categoryId)
        {
            var result = await _productService.ToggleCategoryActiveStatus(categoryId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [HttpGet("milkteas/search")]
        public async Task<IActionResult> SearchMilkteas([FromQuery] string searchTerm)
        {
            var result = await _productService.SearchMilkteas(searchTerm);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [HttpGet("milkteas")]
        public async Task<IActionResult> GetAllMilkteas([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _productService.GetAllMilkteas(queryObject);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Data = result.Data!.Select(mt => mt.ToMilkteaDto()),
                    Total = result.Total,
                    Took = result.Took,
                }
            );
        }

        [HttpGet("milkteas/{milkteaId:int}")]
        public async Task<IActionResult> GetAllMilkteaById([FromRoute] int milkteaId)
        {
            var result = await _productService.GetAllMilkteaById(milkteaId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("milkteas")]
        public async Task<IActionResult> CreateNewMilktea([FromBody] CreateUpdateMilkteaDto createMilkteaDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _productService.CreateNewMilktea(createMilkteaDto);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("milkteas/{milkteaId:int}")]
        public async Task<IActionResult> UpdateMilktea([FromBody] CreateUpdateMilkteaDto updateMilkteaDto, [FromRoute] int milkteaId)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _productService.UpdateMilktea(updateMilkteaDto, milkteaId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("milkteas/toggle-active/{milkteaId:int}")]
        public async Task<IActionResult> ToggleMilkteaActiveStatus([FromRoute] int milkteaId)
        {
            var result = await _productService.ToggleMilkteaActiveStatus(milkteaId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [HttpGet("toppings")]
        public async Task<IActionResult> GetAllToppings([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _productService.GetAllToppings(queryObject);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(
                result.Status,
                new SuccessResponseDto
                {
                    Data = result.Data,
                    Total = result.Total,
                    Took = result.Took,
                }
            );
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("toppings")]
        public async Task<IActionResult> CreateNewTopping([FromBody] CreateUpdateToppingDto createToppingDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _productService.CreateNewTopping(createToppingDto);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("toppings/{toppingId:int}")]
        public async Task<IActionResult> UpdateTopping([FromBody] CreateUpdateToppingDto updateToppingDto, [FromRoute] int toppingId)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var result = await _productService.UpdateTopping(updateToppingDto, toppingId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("toppings/toggle-active/{toppingId:int}")]
        public async Task<IActionResult> ToggleToppingActiveStatus([FromRoute] int toppingId)
        {
            var result = await _productService.ToggleToppingActiveStatus(toppingId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }
    }
}
