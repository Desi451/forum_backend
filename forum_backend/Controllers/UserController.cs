using forum_backend.DTOs;
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

        /// <summary>
        /// Updates nickname of the currently logged in user
        /// </summary>
        /// <param name="nickname">Nickname DTO</param>
        /// <param name="id">User ID</param>
        /// <returns>Success or error message</returns>
        [HttpPatch("update-nickname/{id}")]
        public async Task<IActionResult> UpdateNickname([FromBody] UpdateNicknameDTO nickname, [FromRoute] int id)
        {
            return await _userService.UpdateNickname(nickname, id);
        }

        /// <summary>
        /// Updates e-mail of the currently logged in user
        /// </summary>
        /// <param name="email">E-mail DTO</param>
        /// <param name="id">User ID</param>
        /// <returns>Success or error message</returns>
        [HttpPatch("update-email/{id}")]
        public async Task<IActionResult> UpdateEMail([FromBody] UpdateEMailDTO email, [FromRoute] int id)
        {
            return await _userService.UpdateEMail(email, id);
        }

        /// <summary>
        /// Updates password of the currently logged in user
        /// </summary>
        /// <param name="password">Password DTO</param>
        /// <param name="id">User ID</param>
        /// <returns>Success or error message</returns>
        [HttpPatch("update-password/{id}")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO password, [FromRoute] int id)
        {
            return await _userService.UpdatePassword(password, id);
        }

        /// <summary>
        /// Updates profile picture of the currently logged in user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="profilePicture">Image file</param>
        /// <returns>Success or error message</returns>
        [HttpPatch("update-pfp/{id}")]
        public async Task<IActionResult> AddOrUpdatePFP([FromRoute] int id, [FromForm] IFormFile profilePicture)
        {
            return await _userService.AddOrUpdatePFP(profilePicture, id);
        }

        /// <summary>
        /// Deletes profile picture of the currently logged in user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success or error message</returns>
        [HttpDelete("delete-pfp/{id}")]
        public async Task<IActionResult> DeletePFP([FromRoute] int id)
        {
            return await _userService.DeletePFP(id);
        }

        /// <summary>
        /// Gets data for a specific user
        /// </summary>
        /// <param name="login">User login</param>
        /// <returns>GetUserDTO with data</returns>
        [HttpGet("{login}")]
        public async Task<IActionResult> GetUser([FromRoute] string login)
        {
            return await _userService.GetUser(login);
        }
    }
}