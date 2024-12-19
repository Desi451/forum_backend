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
            nickname.Id = id;
            return await _userService.UpdateNickname(nickname);
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
            email.Id = id;
            return await _userService.UpdateEMail(email);
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
            password.Id = id;
            return await _userService.UpdatePassword(password);
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
        /// <param name="id">User id</param>
        /// <returns>GetUserDTO with data</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            return await _userService.GetUser(id);
        }

        /// <summary>
        /// Gets user profile-picture
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>File or null</returns>
        [HttpGet("{id}/profile-picture")]
        public async Task<IActionResult> GetUserProfilePicture([FromRoute] int id)
        {
            return await _userService.GetUserProfilePictureUrl(id);
        }
    }
}