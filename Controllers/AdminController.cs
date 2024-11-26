using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using milktea_server.Queries;

namespace milktea_server.Controllers
{
    [ApiController]
    [Route("/admins")]
    public class AdminController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<IActionResult> GetAllAdmins([FromQuery] BaseQueryObject queryObject)
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("")]
        public async Task<IActionResult> AddNewAdmin()
        {
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("profile")]
        public async Task<IActionResult> UpdateAdminProfile()
        {
            return Ok();
        }
    }
}
