using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using milktea_server.Dtos.Order;
using milktea_server.Dtos.Response;
using milktea_server.Extensions.Mappers;
using milktea_server.Interfaces.Services;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Controllers
{
    [ApiController]
    [Route("/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto, [FromQuery] string locale)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _orderService.CreateOrder(createOrderDto, int.Parse(authUserId!), locale);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetCustomerOrders([FromQuery] BaseQueryObject queryObject)
        {
            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _orderService.GetCustomerOrders(int.Parse(authUserId!), queryObject.Sort);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data!.Select(or => or.ToOrderDto()) });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _orderService.GetAllOrders(queryObject);
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
                        Orders = result.Data!.Select(or => or.ToOrderDto()),
                        result.Total,
                        result.Took,
                    },
                }
            );
        }

        [Authorize]
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderDetail([FromRoute] int orderId)
        {
            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var authUserRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var result = await _orderService.GetOrderDetail(orderId, int.Parse(authUserId!), authUserRole!);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = new { Order = result.Data!.ToOrderDto() } });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("status/{orderId:int}")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int orderId, [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _orderService.UpdateOrderStatus(orderId, updateOrderStatusDto, int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }
    }
}
