using forum_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> UpdateNickname(UpdateNicknameDTO nickname, int id);
        Task<IActionResult> UpdateEMail(UpdateEMailDTO email, int id);
        Task<IActionResult> UpdatePassword(UpdatePasswordDTO password, int id);
        Task<IActionResult> UpdatePFP(UpdatePFPDTO pfp, int id);
        Task<IActionResult> GetUser(string login);
    }
}