using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using milktea_server.Dtos.Response;
using milktea_server.Interfaces.Services;
using milktea_server.Mappers;
using milktea_server.Queries;

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
                    Data = new
                    {
                        Categories = result.Data,
                        result.Total,
                        result.Took,
                    },
                }
            );
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
                    Data = new
                    {
                        Milkteas = result.Data!.Select(mt => mt.ToMilkteaDto()),
                        result.Total,
                        result.Took,
                    },
                }
            );
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
                    Data = new
                    {
                        Toppings = result.Data,
                        result.Total,
                        result.Took,
                    },
                }
            );
        }
    }
}
