using forum_backend.Database.Context;
using forum_backend.DTOs;
using forum_backend.Entities;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace forum_backend.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthorizationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Register(UserDTO user)
        {
            if (!ValidateEmail(user.EMail) || string.IsNullOrWhiteSpace(user.EMail))
            {
                return new BadRequestObjectResult(new { error = "InvalidEmail",
                    message = "Invalid email address." });
            }

            if (await _context.Users.AnyAsync(x => x.EMail.Equals(user.EMail)) 
                || await _context.Users.AnyAsync(x => x.Login.Equals(user.Login)))
            {
                return new BadRequestObjectResult( new { error = "EmailOrLoginAlreadyExist", 
                    message = "User with the login name or email address you are using already exists." });
            }

            if (!ValidateLoginOrNickname(user.Login) || string.IsNullOrWhiteSpace(user.Login))
            {
                return new BadRequestObjectResult(new { error = "InvalidLogin",
                    message = "Your login must be between 5 and 12 characters long and can only consist of letters and numbers." });
            }

            if (!ValidatePassword(user.Password) || string.IsNullOrWhiteSpace(user.Password))
            {
                return new BadRequestObjectResult(new { error = "InvalidPassword",
                    message = "The password cannot be too short (min. 8 characters) and must contain letters and numbers."
                });
            }

            user.Password = HashPassword(user.Password);

            try
            {
                var newUser = new Users
                {
                    Nickname = user.Login,
                    Login = user.Login,
                    Password = user.Password,
                    EMail = user.EMail,
                    CreationDate = DateTime.Now,
                    ProfilePicture = null,
                    Role = 0,
                    status = 0
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { message = "User has been successfully registered." });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { error = ex, 
                    message = "An error occurred while registering the user." });
            }
        }

        private static string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, 10000, HashAlgorithmName.SHA256);

            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashWithSalt = new byte[36];
            Array.Copy(salt, 0, hashWithSalt, 0, 16);
            Array.Copy(hash, 0, hashWithSalt, 16, 20);

            return Convert.ToBase64String(hashWithSalt);
        }

        private static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^(?!\.)[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }

        private static bool ValidateLoginOrNickname(string login)
        {
            return Regex.IsMatch(login, "^[a-zA-Z0-9]{5,12}$");
        }

        private static bool ValidatePassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }
    }
}
