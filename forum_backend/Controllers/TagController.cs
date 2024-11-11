using forum_backend.Database.Context;
using forum_backend.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace forum_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet(Name = "Tags")]
    public async Task<List<Tag>> Get()
    {
       return await _context.Tags.ToListAsync();
    }
}
