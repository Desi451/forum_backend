using forum_backend.Database.Context;
using forum_backend.DTOs;
using forum_backend.Interfaces;
using forum_backend.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace forum_backend.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> UpdateNickname(UpdateNicknameDTO nickname, int id)
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

            // Sprawdzanie ID, tego nie jestem pewien (jest w każdej metodzie bo jak próbowałem osobno to pojawiał mi się wkurzający warning XD)
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString().Equals(userIdFromToken));

            if (user == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotFound",
                    message = "User not found."
                });
            }

            user.Nickname = nickname.NewNickname;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Nickname successfully changed." });
        }

        public async Task<IActionResult> UpdateEMail(UpdateEMailDTO email, int id)
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

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString().Equals(userIdFromToken));

            if (user == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotFound",
                    message = "User not found."
                });
            }

            if (!ValidationHelper.ValidateEmail(email.NewEMail) || string.IsNullOrWhiteSpace(email.NewEMail))
            {
                return new BadRequestObjectResult(new
                {
                    error = "InvalidEmail",
                    message = "Invalid email address."
                });
            }

            var existingLogin= await _context.Users.FirstOrDefaultAsync(x => x.Login.Equals(email.NewEMail));

            if (existingLogin != null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "EMailTaken",
                    message = "This email is already taken."
                });
            }

            user.Login = email.NewEMail;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Login successfully changed." });
        }

        public async Task<IActionResult> UpdatePassword(UpdatePasswordDTO password, int id)
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

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString().Equals(userIdFromToken));

            if (user == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotFound",
                    message = "User not found."
                });
            }

            if (!ValidationHelper.ValidatePassword(password.NewPassword) || string.IsNullOrWhiteSpace(password.NewPassword))
            {
                return new BadRequestObjectResult(new
                {
                    error = "InvalidPassword",
                    message = "The password cannot be too short (min. 8 characters) and must contain letters and numbers."
                });
            }

            if (PasswordHelper.CheckPassword(password.NewPassword, user.Password))
            {
                return new BadRequestObjectResult(new
                {
                    error = "SamePassword",
                    message = "The new password can't be the same as old."
                });
            }

            if (!PasswordHelper.CheckPassword(password.OldPassword, user.Password))
            {
                return new BadRequestObjectResult(new
                {
                    error = "IncorrectPassword",
                    message = "The old password is incorrect."
                });
            }

            user.Password = PasswordHelper.HashPassword(password.NewPassword);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Password successfully changed." });
        }

        public async Task<IActionResult> UpdatePFP(UpdatePFPDTO pfp, int id)
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

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.ToString().Equals(userIdFromToken));

            if (user == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotFound",
                    message = "User not found."
                });
            }

            if (pfp.RemoveProfilePicture)
            {
                user.ProfilePicture = null;
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { message = "Profile picture successfully removed." });
            }

            if (!string.IsNullOrEmpty(pfp.NewProfilePicture))
            {
                user.ProfilePicture = pfp.NewProfilePicture;
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { message = "Profile picture successfully changed." });
            }

            return new BadRequestObjectResult(new
            {
                error = "InvalidData",
                message = "No profile picture data provided."
            });
        }
    }
}