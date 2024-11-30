using forum_backend.DTOs;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Creates a user account and adds data to the database
        /// </summary>
        /// <param name="user">Register DTO</param>
        /// <returns>Success or error message</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO user)
        {
            return await _authService.Register(user);
        }

        /// <summary>
        /// Creates JWToken
        /// </summary>
        /// <param name="login">Login DTO</param>
        /// <returns>Error message or Token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            return await _authService.Login(login);
        }

        /// <summary>
        /// Refreshes Token
        /// </summary>
        /// <returns>Error message or new Token</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            return await _authService.RefreshToken();
        }
    }
}