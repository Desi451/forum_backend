using forum_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> UpdateNickname(UpdateNicknameDTO nickname);
        Task<IActionResult> UpdateLogin(UpdateLoginDTO login);
        Task<IActionResult> UpdatePassword(UpdatePasswordDTO password);
        Task<IActionResult> UpdatePFP(UpdatePFPDTO pfp);
    }
}
