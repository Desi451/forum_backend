﻿using forum_backend.DTOs;
using forum_backend.Interfaces;
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

        [HttpPatch("update-nickname/{id}")]
        public async Task<IActionResult> UpdateNickname([FromBody] UpdateNicknameDTO nickname, [FromRoute] int id)
        {
            return await _userService.UpdateNickname(nickname, id);
        }

        [HttpPatch("update-email/{id}")]
        public async Task<IActionResult> UpdateEMail([FromBody] UpdateEMailDTO email, [FromRoute] int id)
        {
            return await _userService.UpdateEMail(email, id);
        }

        [HttpPatch("update-password/{id}")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO password, [FromRoute] int id)
        {
            return await _userService.UpdatePassword(password, id);
        }

        [HttpPatch("update-pfp/{id}")]
        public async Task<IActionResult> UpdatePFP([FromBody] UpdatePFPDTO pfp, [FromRoute] int id)
        {
            return await _userService.UpdatePFP(pfp, id);
        }
    }
}