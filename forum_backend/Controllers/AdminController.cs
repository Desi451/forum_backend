using forum_backend.Entities;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("ban-user/{userId}")]
        public async Task<IActionResult> BanUser([FromRoute] int userId)
        {
            return await _adminService.BanUser(userId);
        }

        [HttpPatch("unban-user/{userId}")]
        public async Task<IActionResult> UnbanUser([FromRoute] int userId)
        {
            return await _adminService.UnbanUser(userId);
        }

        [HttpGet("banned-users")]
        public async Task<IActionResult> GetBannedUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 15)
        {
            return await _adminService.GetBannedUsers(pageNumber, pageSize);
        }

        [HttpPost("report-user/{userId}")]
        public async Task<IActionResult> ReportUser([FromRoute] int userId)
        {
            return await _adminService.ReportUser(userId);
        }

        [HttpDelete("delete-report/{userId}")]
        public async Task<IActionResult> DeleteReport([FromRoute] int userId)
        {
            return await _adminService.DeleteReport(userId);
        }

        [HttpGet("reported-user")]
        public async Task<IActionResult> GetReportedUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 15)
        {
            return await _adminService.GetReportedUsers(pageNumber, pageSize);
        }
    }
}
