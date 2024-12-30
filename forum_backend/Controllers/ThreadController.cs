using forum_backend.DTOs;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace forum_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreadController : ControllerBase
    {
        private readonly IThreadService _threadService;
        public ThreadController(IThreadService threadService)
        {
            _threadService = threadService;
        }

        /// <summary>
        /// Creates a new thread and adds it to the database
        /// </summary>
        /// <param name="thread">Thread DTO</param>
        /// <returns>Success or error message</returns>
        [HttpPost("create-thread")]
        public async Task<IActionResult> CreateThread([FromForm] CreateThreadDTO thread)
        {
            return await _threadService.CreateThread(thread);
        }

        /// <summary>
        /// Edits an existing thread
        /// </summary>
        /// <param name="threadId">Edited thread ID</param>
        /// <param name="editThreadDTO">Edited thread DTO</param>
        /// <returns>Success or error message</returns>
        [HttpPatch("edit-thread/{threadId}")]
        public async Task<IActionResult> EditThread([FromRoute] int threadId, [FromBody] EditThreadDTO editThreadDTO)
        {
            return await _threadService.EditThread(threadId, editThreadDTO);
        }

        /// <summary>
        /// Creates a subthread and adds it to the database
        /// </summary>
        /// <param name="parentThreadId">Parent thread ID</param>
        /// <param name="subthreadDTO">Subthread DTO</param>
        /// <returns>Success or error message</returns>
        [HttpPost("create-subthread/{parentThreadId}")]
        public async Task<IActionResult> CreateSubthread([FromRoute] int parentThreadId, [FromBody] CreateSubthreadDTO subthreadDTO)
        {
            return await _threadService.CreateSubthread(parentThreadId, subthreadDTO);
        }

        /// <summary>
        /// Deletes thread
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <returns>Success or error message</returns>
        [HttpPatch("delete-thread/{threadId}")]
        public async Task<IActionResult> DeleteThread([FromRoute] int threadId)
        {
            return await _threadService.DeleteThread(threadId);
        }

        /// <summary>
        /// Manages user like and dislike for a specific thread
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="likeOrDislike">Int (1 -> like; -1 -> dislike)</param>
        /// <returns>Success or error message</returns>
        [HttpPost("like-dislike/{threadId}")]
        public async Task<IActionResult> LikeOrDislikeThread([FromRoute] int threadId, [FromBody] int likeOrDislike)
        {
            return await _threadService.LikeOrDislikeThread(threadId, likeOrDislike);
        }

        /// <summary>
        /// Manages user subscription for a specific thread 
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <returns>Success or error message</returns>
        [HttpPost("subscribe/{threadId}")]
        public async Task<IActionResult> SubscribeThread([FromRoute] int threadId)
        {
            return await _threadService.SubscribeThread(threadId);
        }

        /// <summary>
        /// Gets max 15 threads from database
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Number of threads in page</param>
        /// <returns>List of threads</returns>
        [HttpGet("threads")]
        public async Task<IActionResult> GetThreads([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 15)
        {
            return await _threadService.GetThreads(pageNumber, pageSize);
        }

        /// <summary>
        /// Gets max 15 subscribed threads from database
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Number of threads in page</param>
        /// <returns>List of subscribed threads</returns>
        [HttpGet("subscriptions")]
        public async Task<IActionResult> GetSubscribedThreads([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 15)
        {
            return await _threadService.GetSubscribedThreads(pageNumber, pageSize);
        }

        /// <summary>
        /// Gets main thread with subthreads
        /// </summary>
        /// <param name="id">Thread ID</param>
        /// <returns>Main thread with nested threads</returns>
        [HttpGet("thread/{id}")]
        public async Task<IActionResult> GetThreadAndSubthreads([FromRoute] int id)
        {
            return await _threadService.GetThreadAndSubthreads(id);
        }

        /// <summary>
        /// Searches for threads based on a given word (max 15)
        /// </summary>
        /// <param name="keyWord">Key word</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Number of threads in page</param>
        /// <returns>List of threads</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchThread([FromQuery] string keyWord, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 15)
        {
            return await _threadService.SearchThread(keyWord, pageNumber, pageSize);
        }

        /// <summary>
        /// Gets thread image
        /// </summary>
        /// <param name="threadId">Thread ID</param>
        /// <param name="filePath">File path</param>
        /// <returns>File or null</returns>
        [HttpGet("threads/{threadId}/images/{filePath}")]
        public async Task<IActionResult> GetThreadImage([FromQuery] int threadId, [FromQuery] string filePath)
        {
            return await _threadService.GetThreadImage(threadId, filePath);
        }
    }
}