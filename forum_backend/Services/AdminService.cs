using forum_backend.Database.Context;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdminService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> BanUser(int userId)
        {
            return null;
        }

        public async Task<IActionResult> UnbanUser(int userId)
        {
            return null;
        }

        public async Task<IActionResult> GetBannedUsers(int pageNumber, int pageSize)
        {
            return null;
        }

        public async Task<IActionResult> ReportUser(int userId)
        {
            return null;
        }

        public async Task<IActionResult> DeleteReport(int userId)
        {
            return null;
        }

        public async Task<IActionResult> GetReportedUsers(int pageNumber, int pageSize)
        {
            return null;
        }
    }
}
