using forum_backend.Database.Context;
using forum_backend.DTOs;
using forum_backend.Entities;
using forum_backend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace forum_backend.Services
{
    public class ThreadService : IThreadService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ThreadService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> CreateThread(CreateThreadDTO thread)
        {
            var userIdFromToken = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;

            if (userIdFromToken == null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in."
                });
            }

            var errors = new List<object>();

            if (string.IsNullOrWhiteSpace(thread.Title) || thread.Title.Length < 5 || thread.Title.Length > 100)
            {
                errors.Add(new
                {
                    error = "InvalidTitle",
                    message = "The title must be between 5 and 100 characters long."
                });
            }

            if (string.IsNullOrWhiteSpace(thread.Description) || thread.Description.Length < 10 || thread.Description.Length > 1000)
            {
                errors.Add(new
                {
                    error = "InvalidDescription",
                    message = "The description must be between 10 and 1000 characters long."
                });
            }

            if (thread.Images.Count > 5)
            {
                errors.Add(new
                {
                    error = "TooManyImages",
                    message = "You can only upload up to 5 images."
                });
            }
            else
            {
                foreach (var image in thread.Images)
                {
                    if (image.Length > 5 * 1024 * 1024)
                    {
                        errors.Add(new
                        {
                            error = "FileTooLarge",
                            message = $"The file '{image.FileName}' exceeds the size limit of 5 MB."
                        });
                    }

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(image.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                    {
                        errors.Add(new
                        {
                            error = "InvalidFileType",
                            message = $"The file '{image.FileName}' has an unsupported extension. Allowed: {string.Join(", ", allowedExtensions)}"
                        });
                    }
                }
            }

            if (thread.Tags.Count > 10)
            {
                errors.Add(new
                {
                    error = "TooManyTags",
                    message = "You can only add up to 10 tags."
                });
            }

            if (errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            var newThread = new Threads
            {
                Title = thread.Title,
                Description = thread.Description,
                AuthorId = int.Parse(userIdFromToken),
                CreationDate = DateTimeOffset.Now,
                Deleted = false
            };

            _context.Threads.Add(newThread);
            await _context.SaveChangesAsync();

            var threadFolder = Path.Combine("Images", "Threads", newThread.Id.ToString());
            if (!Directory.Exists(threadFolder))
            {
                Directory.CreateDirectory(threadFolder);
            }

            var threadImages = new List<Images>();

            foreach (var image in thread.Images)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(threadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                threadImages.Add(new Images
                {
                    ThreadId = newThread.Id,
                    Image = filePath
                });
            }

            _context.Images.AddRange(threadImages);

            var threadTags = new List<ThreadTags>();

            foreach (var tagName in thread.Tags.Distinct())
            {
                var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Tag == tagName);

                if (existingTag == null)
                {
                    existingTag = new Tags { Tag = tagName };
                    _context.Tags.Add(existingTag);
                    await _context.SaveChangesAsync();
                }

                threadTags.Add(new ThreadTags
                {
                    ThreadId = newThread.Id,
                    TagId = existingTag.Id
                });
            }

            _context.ThreadTags.AddRange(threadTags);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Thread created successfully." });
        }

        public async Task<IActionResult> GetThreads(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult("Page number and size must be greater than zero.");
            }

            var threadsQuery = _context.Threads
                .Include(t => t.Author)
                .Include(t => t.ThreadTags!.ToList())
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.ThreadImages)
                .Where(t => !t.Deleted)
                .OrderByDescending(t => t.CreationDate);

            var totalCount = await threadsQuery.CountAsync();

            var threads = await threadsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var threadDTOs = threads.Select(t => new GetThreadsDTO
            {
                ThreadId = t.Id,
                Title = t.Title,
                AuthorId = t.AuthorId,
                Author = t.Author.Nickname,
                Description = t.Description,
                CreationDate = t.CreationDate,
                Tags = t.ThreadTags != null && t.ThreadTags.Any() ? t.ThreadTags.Select(tt => tt.Tag.Tag).ToList() : null,
                Image = t.ThreadImages != null && t.ThreadImages.Any() ? t.ThreadImages.FirstOrDefault()?.Image : null
            }).ToList();

            return new OkObjectResult(new
            {
                Data = threadDTOs,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                CurrentPage = pageNumber,
                PageSize = pageSize
            });
        }
    }
}