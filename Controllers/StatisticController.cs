using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace milktea_server.Controllers
{
    [ApiController]
    [Route("/statistic")]
    public class StatisticController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<IActionResult> GetOverallStatistic()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/popular")]
        public async Task<IActionResult> GetPopularStatistic()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/revenues")]
        public async Task<IActionResult> GetRevenuesStatistic()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/products/{productId:int}")]
        public async Task<IActionResult> GetProductStatistic()
        {
            return Ok();
        }
    }
}
