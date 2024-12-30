using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IAdminService
    {
        Task<IActionResult> BanUser(int userId);
        Task<IActionResult> UnbanUser(int userId);
        Task<IActionResult> GetBannedUsers(int pageNumber, int pageSize);
        Task<IActionResult> ReportUser(int userId);
        Task<IActionResult> DeleteReport(int userId);
        Task<IActionResult> GetReportedUsers(int pageNumber, int pageSize);
    }
}
