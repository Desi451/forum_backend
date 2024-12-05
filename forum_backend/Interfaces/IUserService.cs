using forum_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> UpdateNickname(UpdateNicknameDTO nickname);
        Task<IActionResult> UpdateEMail(UpdateEMailDTO email);
        Task<IActionResult> UpdatePassword(UpdatePasswordDTO password);
        Task<IActionResult> AddOrUpdatePFP(IFormFile pfp, int id);
        Task<IActionResult> DeletePFP(int id);
        Task<IActionResult> GetUser(int id);
        Task<IActionResult> GetUserProfilePicture(int id);
    }
}