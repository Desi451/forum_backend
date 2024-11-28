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

        [HttpPost("create-thread")]
        public async Task<IActionResult> CreateThread([FromBody] CreateThreadDTO thread)
        {
            return await _threadService.CreateThread(thread);
        }
    }
}
