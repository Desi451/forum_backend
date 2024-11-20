using forum_backend.Entities;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IAuthorizationService
    {
        Task<IActionResult> Register(Users user);
    }
}
