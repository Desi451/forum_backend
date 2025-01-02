using forum_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IAdminService
    {
        Task<IActionResult> BanUser(int userId, BanUserDTO banData);
        Task<IActionResult> UnbanUser(int userId);
        Task<IActionResult> GetBannedUsers(int pageNumber, int pageSize);
        Task<IActionResult> ReportUser(int userId, string reason);
        Task<IActionResult> DeleteReport(int reportId);
        Task<IActionResult> GetReportedUsers(int pageNumber, int pageSize);
    }
}
