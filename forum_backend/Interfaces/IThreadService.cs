using forum_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Interfaces
{
    public interface IThreadService
    {
        Task<IActionResult> CreateThread(CreateThreadDTO thread);
        Task<IActionResult> EditThread(int threadId, EditThreadDTO editThreadDTO);
        Task<IActionResult> CreateSubthread(int parentThreadId, CreateSubthreadDTO subthreadDTO);
        Task<IActionResult> DeleteThread(int threadId);
        Task<IActionResult> LikeOrDislikeThread(int threadId, int likeOrDislike);
        Task<IActionResult> SubscribeThread(int threadId);
        Task<IActionResult> GetThreads(int pageNumber, int pageSize);
        Task<IActionResult> GetUserThreads(int userId, int pageNumber, int pageSize);
        Task<IActionResult> GetMostDislikedThreads(int pageNumber, int pageSize);
        Task<IActionResult> GetTopLikedThreads();
        Task<IActionResult> GetSubscribedThreads(int pageNumber, int pageSize);
        Task<IActionResult> GetThreadAndSubthreads(int id);
        Task<IActionResult> SearchThread(string keyWord, int pageNumber, int pageSize);
        Task<IActionResult> GetThreadImage(int threadId, string filePath);
    }
}