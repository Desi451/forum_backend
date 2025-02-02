﻿using System.Security.Claims;
using forum_backend.Database.Context;
using forum_backend.DTOs;
using forum_backend.Entities;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> BanUser(int userId, BanUserDTO banData)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in."
                });
            }

            var roleClaim = claimsIdentity.FindFirst("UserRole");
            var adminIdClaim = claimsIdentity.FindFirst("UserID");

            if (roleClaim == null || adminIdClaim == null || roleClaim.Value != "1")
                return new UnauthorizedResult();

            int adminId = int.Parse(adminIdClaim.Value);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult( new { message = "User not found!" });

            var existingBan = await _context.Bans.FirstOrDefaultAsync(b => b.BannedUserId == userId);
            if (existingBan != null)
                return new ConflictObjectResult( new { message = "User is already banned!" });

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var threads = await _context.Threads.Where(t => t.AuthorId == userId).ToListAsync();
                threads.ForEach(t => t.Deleted = true);
                _context.Threads.UpdateRange(threads);

                var reports = await _context.Reports
                    .Where(r => r.ReportedUserId == userId || r.ReportingUserId == userId)
                    .ToListAsync();
                _context.Reports.RemoveRange(reports);

                user.status = -1;
                _context.Users.Update(user);

                var ban = new Bans
                {
                    BannedUserId = userId,
                    BanningModeratorId = adminId,
                    Reason = banData.Reason,
                    BanDate = DateTimeOffset.UtcNow,
                    BanUntil = banData.BannedUntil
                };

                await _context.Bans.AddAsync(ban);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OkObjectResult(new { message = "User banned successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.Write(ex.ToString());
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IActionResult> UnbanUser(int userId)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in."
                });
            }

            var roleClaim = claimsIdentity.FindFirst("UserRole");
            if (roleClaim == null || roleClaim.Value != "1")
                return new UnauthorizedResult();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult(new { message = "User not found!" });

            var ban = await _context.Bans.FirstOrDefaultAsync(b => b.BannedUserId == userId);
            if (ban == null)
                return new BadRequestObjectResult(new { message = "User is not banned!" });

            user.status = 0;
            _context.Users.Update(user);

            try
            {
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { message = "User unbanned successfully." });
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IActionResult> GetBannedUsers(int pageNumber, int pageSize)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return new UnauthorizedResult();

            var roleClaim = claimsIdentity.FindFirst("UserRole");
            if (roleClaim == null || roleClaim.Value != "1")
                return new UnauthorizedResult();

            var userIdClaim = claimsIdentity.FindFirst("UserID");
            if (userIdClaim == null)
                return new ForbidResult("Matrix error.");

            if (pageNumber <= 0 || pageSize <= 0)
                return new BadRequestObjectResult(new { message = "Page number and page size must be greater than zero." });

            var skip = (pageNumber - 1) * pageSize;

            var totalCount = await _context.Bans.Where(b => b.BannedUser.status == -1).CountAsync();

            var bannedUsers = await _context.Bans
                .Include(b => b.BannedUser)
                .Include(b => b.BanningModerator)
                .Where(b => b.BannedUser.status == -1)
                .Skip(skip)
                .Take(pageSize)
                .Select(b => new GetBannedUsersDTO
                {
                    BannedUserId = b.BannedUserId,
                    BannedUserNickname = b.BannedUser.Nickname,
                    BannedUserLogin = b.BannedUser.Login,
                    BannedUserEMail = b.BannedUser.EMail,
                    Reason = b.Reason,
                    DateOfBan = b.BanDate,
                    BannedUntil = b.BanUntil,
                    AdminId = b.BanningModeratorId,
                    AdminNickname = b.BanningModerator.Nickname,
                    AdminLogin = b.BanningModerator.Login
                })
                .ToListAsync();

            if (!bannedUsers.Any())
            {
                return new NotFoundObjectResult(new { message = "WoW! No one is banned!" });
            }

            return new OkObjectResult(new
            {
                Data = bannedUsers,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                CurrentPage = pageNumber,
                PageSize = pageSize

            });
        }

        public async Task<IActionResult> ReportUser(int userId, string reason)
        {
            var userIdFromToken = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;

            if (userIdFromToken == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in."
                });
            }

            int reportingUserId = int.Parse(userIdFromToken);

            var reportedUser = await _context.Users.FindAsync(userId);
            if (reportedUser == null)
                return new NotFoundObjectResult(new { message = "User not found!" });

            if (reportedUser.status == -1)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserAlreadyBanned",
                    message = "User is already banned!"
                });
            }

            var report = new Reports
            {
                ReportedUserId = userId,
                ReportingUserId = reportingUserId,
                Reason = reason,
                ReportDate = DateTimeOffset.UtcNow
            };

            try
            {
                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { message = "We have received your report!" });
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IActionResult> DeleteReport(int reportId)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return new UnauthorizedResult();

            var roleClaim = claimsIdentity.FindFirst("UserRole");
            if (roleClaim == null || roleClaim.Value != "1")
                return new UnauthorizedResult();

            var adminIdClaim = claimsIdentity.FindFirst("UserID");
            if (adminIdClaim == null)
                return new ForbidResult("Matrix error.");

            var report = await _context.Reports.FindAsync(reportId);
            if (report == null)
                return new NotFoundObjectResult(new { message = "Report not found!" });

            var reportedUser = await _context.Users.FindAsync(report.ReportedUserId);
            if (reportedUser == null)
                return new NotFoundObjectResult(new { message = "Report user not found!" });

            if (reportedUser.status == -1)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserAlreadyBanned",
                    message = "User is already banned!"
                });
            }

            _context.Reports.Remove(report);

            try
            {
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { message = "Report has been deleted!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<IActionResult> GetReportedUsers(int pageNumber, int pageSize)
        {
            var claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
                return new UnauthorizedResult();

            var roleClaim = claimsIdentity.FindFirst("UserRole");
            if (roleClaim == null || roleClaim.Value != "1")
                return new UnauthorizedResult();

            var userIdClaim = claimsIdentity.FindFirst("UserID");
            if (userIdClaim == null)
                return new ForbidResult("Matrix error.");

            if (pageNumber <= 0 || pageSize <= 0)
                return new BadRequestObjectResult(new { message = "Page number and page size must be greater than zero." });

            var skip = (pageNumber - 1) * pageSize;

            var totalCount = await _context.Reports.CountAsync();

            var reports = await _context.Reports
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportingUser)
                .Skip(skip)
                .Take(pageSize)
                .Select(r => new GetReportedUserDTO
                {
                    ReportId = r.Id,
                    ReportedUserId = r.ReportedUserId,
                    ReportedUserNickname = r.ReportedUser.Nickname,
                    ReportedUserLogin = r.ReportedUser.Login,
                    ReportedUserMail = r.ReportedUser.EMail,
                    Reason = r.Reason,
                    ReportDate = r.ReportDate,
                    ReportingUserId = r.ReportingUserId,
                    ReportingUserNickname = r.ReportingUser.Nickname,
                    ReportingUserLogin = r.ReportingUser.Login,
                    ReportingUserMail = r.ReportingUser.EMail
                })
                .ToListAsync();

            if (!reports.Any())
            {
                return new NotFoundObjectResult(new { message = "WoW! No one is reported!" });
            }

            return new OkObjectResult(new
            {
                Data = reports,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                CurrentPage = pageNumber,
                PageSize = pageSize

            });
        }
    }
}
