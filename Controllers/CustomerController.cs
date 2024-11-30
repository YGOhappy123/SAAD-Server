using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using milktea_server.Dtos.Order;
using milktea_server.Dtos.Response;
using milktea_server.Dtos.User;
using milktea_server.Extensions.Mappers;
using milktea_server.Interfaces.Services;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Controllers
{
    [ApiController]
    [Route("/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly IUserService _userService;

        public CustomerController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<IActionResult> GetAllCustomers([FromQuery] BaseQueryObject queryObject)
        {
            var result = await _userService.GetAllCustomers(queryObject);
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

        [Authorize(Roles = "Customer")]
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateCustomerDto updateCustomerDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _userService.UpdateCustomerProfile(updateCustomerDto, int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("cart/get-items")]
        public async Task<IActionResult> GetCartItems()
        {
            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _userService.GetCustomerCartItems(int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data!.Select(ci => ci.ToCartItemDto()) });
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("cart/add-item")]
        public async Task<IActionResult> AddCartItem([FromBody] AddCartItemDto addCartItemDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _userService.AddCartItem(addCartItemDto, int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Customer")]
        [HttpPatch("cart/update-item/{itemId:int}")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto updateCartItemDto, [FromRoute] int itemId)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _userService.UpdateCartItem(updateCartItemDto, itemId, int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Customer")]
        [HttpDelete("cart/remove-item/{itemId:int}")]
        public async Task<IActionResult> DeleteCartItem([FromRoute] int itemId)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(
                    ResStatusCode.UNPROCESSABLE_ENTITY,
                    new ErrorResponseDto { Message = ErrorMessage.DATA_VALIDATION_FAILED }
                );
            }

            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _userService.RemoveCartItem(itemId, int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("cart/reset")]
        public async Task<IActionResult> ResetCart()
        {
            var authUserId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var result = await _userService.ResetCustomerCart(int.Parse(authUserId!));
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Message = result.Message });
        }
    }
}
