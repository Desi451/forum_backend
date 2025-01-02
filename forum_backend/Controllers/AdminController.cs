using forum_backend.DTOs;
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

        /// <summary>
        /// Sets and creates data for banned user
        /// </summary>
        /// <param name="userId">Banned user ID</param>
        /// <param name="banData">Ban DTO</param>
        /// <returns>Success or error message</returns>
        [HttpPost("ban-user/{userId}")]
        public async Task<IActionResult> BanUser([FromRoute] int userId, [FromBody] BanUserDTO banData)
        {
            return await _adminService.BanUser(userId, banData);
        }

        /// <summary>
        /// Manually unbanning user
        /// </summary>
        /// <param name="userId">Banned user ID</param>
        /// <returns>Success or error message</returns>
        [HttpPatch("unban-user/{userId}")]
        public async Task<IActionResult> UnbanUser([FromRoute] int userId)
        {
            return await _adminService.UnbanUser(userId);
        }

        /// <summary>
        /// Gets max 20 banned users from database
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Number of banned users in page</param>
        /// <returns>List of banned users</returns>
        [HttpGet("banned-users")]
        public async Task<IActionResult> GetBannedUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            return await _adminService.GetBannedUsers(pageNumber, pageSize);
        }

        /// <summary>
        /// Creates data for reported user
        /// </summary>
        /// <param name="userId">Reported user ID</param>
        /// <param name="reason">Reason</param>
        /// <returns>Success or error message</returns>
        [HttpPost("report-user/{userId}")]
        public async Task<IActionResult> ReportUser([FromRoute] int userId, [FromBody] string reason)
        {
            return await _adminService.ReportUser(userId, reason);
        }

        /// <summary>
        /// Manually deleting report
        /// </summary>
        /// <param name="reportId">Report ID</param>
        /// <returns>Success or error message</returns>
        [HttpDelete("delete-report/{reportId}")]
        public async Task<IActionResult> DeleteReport([FromRoute] int reportId)
        {
            return await _adminService.DeleteReport(reportId);
        }

        /// <summary>
        /// Gets max 20 reported users from database
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Number of banned users in page</param>
        /// <returns>List of reported users</returns>
        [HttpGet("reported-users")]
        public async Task<IActionResult> GetReportedUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            return await _adminService.GetReportedUsers(pageNumber, pageSize);
        }
    }
}
