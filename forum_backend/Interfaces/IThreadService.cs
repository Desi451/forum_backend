using forum_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IThreadService
    {
        Task<IActionResult> CreateThread(CreateThreadDTO thread);
        Task<IActionResult> GetThreads(int pageNumber, int pageSize);
        Task<IActionResult> SearchThread(string keyWord, int pageNumber, int pageSize);
    }
}