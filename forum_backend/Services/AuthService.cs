using forum_backend.Database.Context;
using forum_backend.DTOs;
using forum_backend.Entities;
using forum_backend.Interfaces;
using forum_backend.Utilities;
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
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Register(RegisterDTO user)
        {
            if (!ValidationHelper.ValidateEmail(user.EMail) || string.IsNullOrWhiteSpace(user.EMail))
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

            if (!ValidationHelper.ValidateLoginOrNickname(user.Login) || string.IsNullOrWhiteSpace(user.Login))
            {
                return new BadRequestObjectResult(new
                {
                    error = "InvalidLogin",
                    message = "Your login must be between 5 and 12 characters long and can only consist of letters and numbers."
                });
            }

            if (!ValidationHelper.ValidatePassword(user.Password) || string.IsNullOrWhiteSpace(user.Password))
            {
                return new BadRequestObjectResult(new
                {
                    error = "InvalidPassword",
                    message = "The password cannot be too short (min. 8 characters) and must contain letters and numbers."
                });
            }

            user.Password = PasswordHelper.HashPassword(user.Password);

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
                return new BadRequestObjectResult(new
                {
                    error = ex, 
                    message = "An error occurred while registering the user."
                });
            }
        }

        public async Task<IActionResult> Login(LoginDTO login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.EMail.Equals(login.LoginOrEMail) || x.Login.Equals(login.LoginOrEMail));

            if (user == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "IncorrectLoginOrEmail",
                    message = "Incorrect email or login."
                });
            }

            if (!PasswordHelper.CheckPassword(login.Password, user.Password))
            {
                return new BadRequestObjectResult(new
                {
                    error = "IncorrectPasssword",
                    message = "Incorrect password."
                });
            }

            var token = JWTGenerator(user);
            return new OkObjectResult(new { token });
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