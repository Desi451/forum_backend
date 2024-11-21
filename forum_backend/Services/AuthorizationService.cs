using forum_backend.Database.Context;
using forum_backend.DTOs;
using forum_backend.Entities;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                return new BadRequestObjectResult(new
                {
                    error = "InvalidEmail",
                    message = "Invalid email address."
                });
            }

            if (await _context.Users.AnyAsync(x => x.EMail.Equals(user.EMail))
                || await _context.Users.AnyAsync(x => x.Login.Equals(user.Login)))
            {
                return new BadRequestObjectResult(new
                {
                    error = "EmailOrLoginAlreadyExist",
                    message = "User with the login name or email address you are using already exists."
                });
            }

            if (!ValidateLoginOrNickname(user.Login) || string.IsNullOrWhiteSpace(user.Login))
            {
                return new BadRequestObjectResult(new
                {
                    error = "InvalidLogin",
                    message = "Your login must be between 5 and 12 characters long and can only consist of letters and numbers."
                });
            }

            if (!ValidatePassword(user.Password) || string.IsNullOrWhiteSpace(user.Password))
            {
                return new BadRequestObjectResult(new
                {
                    error = "InvalidPassword",
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

        public async Task<IActionResult> Login(LoginDTO login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.EMail.Equals(login.LoginOrEMail) || x.Login.Equals(login.LoginOrEMail));

            if (user == null)
            {
                return new BadRequestObjectResult(new { error = "IncorrectLoginOrEmail",
                    message = "Incorrect email or login." });
            }

            if (!CheckPassword(login.Password, user.Password))
            {
                return new BadRequestObjectResult(new { error = "IncorrectPasssword",
                    message = "Incorrect password." });
            }

            var token = JWTGenerator(user);
            return new OkObjectResult(new { token });
        }

        private static bool CheckPassword(string enteredPassword, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(enteredPasswordBytes, salt, 10000, HashAlgorithmName.SHA256);

            byte[] enteredHash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != enteredHash[i])
                {
                    return false;
                }
            }

            return true;
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

        private string JWTGenerator(Users user)
        {
            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Key"]!);
            var tokenExpiration = _configuration.GetValue<int>("JwtConfig:TokenExpiration");
            var tokenExpirationTimeStamp = DateTime.UtcNow.AddHours(tokenExpiration);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("UserID", user.Id.ToString()),
                    new Claim("UserNickname", user.Nickname),
                    new Claim("UserLogin", user.Login),
                    new Claim("UserPFP", user.ProfilePicture ?? "TutajBędzieŚcieżkaDoDomyślnegoPFF.png"),
                    new Claim("UserRole", user.Role.ToString())
                ]),
                Expires = tokenExpirationTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}