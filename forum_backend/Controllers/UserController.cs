using forum_backend.DTOs;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("update-nickname")]
        public async Task<IActionResult> UpdateNickname([FromBody] UpdateNicknameDTO nickname)
        {
            return await _userService.UpdateNickname(nickname);
        }

        [HttpPost("update-login")]
        public async Task<IActionResult> UpdateLogin([FromBody] UpdateLoginDTO login)
        {
            return await _userService.UpdateLogin(login);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO password)
        {
            return await _userService.UpdatePassword(password);
        }

        [HttpPost("update-pfp")]
        public async Task<IActionResult> UpdatePFP([FromBody] UpdatePFPDTO pfp)
        {
            return await _userService.UpdatePFP(pfp);
        }
    }
}