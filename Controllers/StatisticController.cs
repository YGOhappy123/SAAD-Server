using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using milktea_server.Dtos.Response;
using milktea_server.Interfaces.Services;

namespace milktea_server.Controllers
{
    [ApiController]
    [Route("/statistic")]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<IActionResult> GetSummaryStatistic([FromQuery] string type)
        {
            var result = await _statisticService.GetSummaryStatistic(type);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularStatistic([FromQuery] string type)
        {
            var result = await _statisticService.GetPopularStatistic(type);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("revenues")]
        public async Task<IActionResult> GetRevenuesChart([FromQuery] string type, [FromQuery] string locale)
        {
            var result = await _statisticService.GetRevenuesChart(type, locale);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("products/{milkteaId:int}")]
        public async Task<IActionResult> GetProductStatistic([FromRoute] int milkteaId)
        {
            var result = await _statisticService.GetProductStatistic(milkteaId);
            if (!result.Success)
            {
                return StatusCode(result.Status, new ErrorResponseDto { Message = result.Message });
            }

            return StatusCode(result.Status, new SuccessResponseDto { Data = result.Data });
        }
    }
}
