using forum_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IAuthService
    {
        Task<IActionResult> Register(UserDTO user);
        Task<IActionResult> Login(LoginDTO login);
    }
}