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
        public async Task<IActionResult> CreateThread([FromBody] CreateThreadDTO thread)
        {
            return await _threadService.CreateThread(thread);
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
    }
}