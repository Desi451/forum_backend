using forum_backend.Database.Context;
using forum_backend.DTOs;
using forum_backend.Entities;
using forum_backend.Interfaces;
using forum_backend.Utilities;
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
                AuthorId = thread.userId,
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

        public async Task<IActionResult> EditThread(int threadId, EditThreadDTO editThreadDTO)
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

            var thread = await _context.Threads
                .Include(t => t.ThreadImages!)
                .Include(t => t.ThreadTags!)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == threadId);

            if (thread == null || thread.Deleted)
            {
                return new NotFoundObjectResult(new
                {
                    error = "ThreadNotFound",
                    message = "Thread not found or deleted."
                });
            }

            if (thread.AuthorId.ToString() != userIdFromToken)
            {
                return new ForbidResult();
            }

            var errors = new List<object>();

            if (thread.PrimeThreadId == null && thread.SupThreadId == null)
            {
                if (!string.IsNullOrWhiteSpace(editThreadDTO.Title) &&
                    (editThreadDTO.Title.Length < 5 || editThreadDTO.Title.Length > 100))
                {
                    errors.Add(new
                    {
                        error = "InvalidTitle",
                        message = "The title must be between 5 and 100 characters long."
                    });
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(editThreadDTO.Title))
                {
                    errors.Add(new
                    {
                        error = "CannotEditTitle",
                        message = "Title cannot be edited for sub-threads."
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(editThreadDTO.Description) &&
                (editThreadDTO.Description.Length < 10 || editThreadDTO.Description.Length > 1000))
            {
                errors.Add(new
                {
                    error = "InvalidDescription",
                    message = "The description must be between 10 and 1000 characters long."
                });
            }

            if (editThreadDTO.Tags != null && editThreadDTO.Tags.Count > 10)
            {
                errors.Add(new
                {
                    error = "TooManyTags",
                    message = "You can only add up to 10 tags."
                });
            }

            if (editThreadDTO.Images != null && editThreadDTO.Images.Count > 5)
            {
                errors.Add(new
                {
                    error = "TooManyImages",
                    message = "You can only upload up to 5 images."
                });
            }

            if (errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            if (thread.PrimeThreadId == null && thread.SupThreadId == null && !string.IsNullOrWhiteSpace(editThreadDTO.Title))
            {
                thread.Title = editThreadDTO.Title;
            }

            if (!string.IsNullOrWhiteSpace(editThreadDTO.Description))
            {
                thread.Description = editThreadDTO.Description;
            }

            if (editThreadDTO.Tags != null)
            {
                var tagsToRemove = thread.ThreadTags?
                    .Where(tt => !editThreadDTO.Tags.Contains(tt.Tag.Tag))
                    .ToList();

                if (tagsToRemove != null)
                {
                    foreach (var tagToRemove in tagsToRemove)
                    {
                        _context.ThreadTags.Remove(tagToRemove);
                    }
                }

                foreach (var tagName in editThreadDTO.Tags.Distinct())
                {
                    var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Tag == tagName);

                    if (existingTag == null)
                    {
                        existingTag = new Tags { Tag = tagName };
                        _context.Tags.Add(existingTag);
                        await _context.SaveChangesAsync();
                    }

                    if (thread.ThreadTags != null && !thread.ThreadTags.Any(tt => tt.TagId == existingTag.Id))
                    {
                        thread.ThreadTags.Add(new ThreadTags
                        {
                            ThreadId = thread.Id,
                            TagId = existingTag.Id
                        });
                    }
                }
            }

            if (editThreadDTO.Images != null && editThreadDTO.Images.Any())
            {
                if (thread.ThreadImages != null)
                {
                    foreach (var oldImage in thread.ThreadImages)
                    {
                        var filePath = oldImage.Image;
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        _context.Images.Remove(oldImage);
                    }
                }

                var threadFolder = Path.Combine("Images", "Threads", thread.Id.ToString());
                if (!Directory.Exists(threadFolder))
                {
                    Directory.CreateDirectory(threadFolder);
                }

                var newImages = new List<Images>();

                foreach (var image in editThreadDTO.Images)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    var filePath = Path.Combine(threadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    newImages.Add(new Images
                    {
                        ThreadId = thread.Id,
                        Image = filePath
                    });
                }

                _context.Images.AddRange(newImages);
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Thread updated successfully." });
        }

        public async Task<IActionResult> CreateSubthread(int parentThreadId, CreateSubthreadDTO subthreadDTO)
        {
            var userIdFromToken = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken) || !int.TryParse(userIdFromToken, out int userId))
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in or your user ID is invalid."
                });
            }

            var errors = new List<object>();

            if (string.IsNullOrWhiteSpace(subthreadDTO.Description) || subthreadDTO.Description.Length < 10 || subthreadDTO.Description.Length > 1000)
            {
                errors.Add(new
                {
                    error = "InvalidDescription",
                    message = "The description must be between 10 and 1000 characters long."
                });
            }

            if (subthreadDTO.Images.Count > 5)
            {
                errors.Add(new
                {
                    error = "TooManyImages",
                    message = "You can only upload up to 5 images."
                });
            }
            else
            {
                foreach (var image in subthreadDTO.Images)
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

            if (errors.Any())
            {
                return new BadRequestObjectResult(errors);
            }

            var parentThread = await _context.Threads
                .Include(t => t.ThreadTags)
                .FirstOrDefaultAsync(t => t.Id == parentThreadId && !t.Deleted);

            if (parentThread == null)
            {
                return new NotFoundObjectResult(new
                {
                    error = "ParentThreadNotFound",
                    message = "The parent thread does not exist or has been deleted."
                });
            }

            int primeThreadId = parentThread.PrimeThreadId ?? parentThread.Id;

            var newSubthread = new Threads
            {
                Title = parentThread.Title,
                Description = subthreadDTO.Description,
                AuthorId = userId,
                CreationDate = DateTimeOffset.Now,
                Deleted = false,
                SupThreadId = parentThreadId,
                PrimeThreadId = primeThreadId
            };

            _context.Threads.Add(newSubthread);
            await _context.SaveChangesAsync();

            var subthreadFolder = Path.Combine("Images", "Threads", newSubthread.Id.ToString());
            if (!Directory.Exists(subthreadFolder))
            {
                Directory.CreateDirectory(subthreadFolder);
            }

            var subthreadImages = new List<Images>();

            foreach (var image in subthreadDTO.Images)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var filePath = Path.Combine(subthreadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                subthreadImages.Add(new Images
                {
                    ThreadId = newSubthread.Id,
                    Image = filePath
                });
            }

            _context.Images.AddRange(subthreadImages);

            if (parentThread.ThreadTags != null && parentThread.ThreadTags.Any())
            {
                var subthreadTags = parentThread.ThreadTags
                    .Select(tt => new ThreadTags
                    {
                        ThreadId = newSubthread.Id,
                        TagId = tt.TagId
                    })
                    .ToList();

                _context.ThreadTags.AddRange(subthreadTags);
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Subthread created successfully." });
        }

        public async Task<IActionResult> DeleteThread(int threadId)
        {
            var userIdFromToken = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken) || !int.TryParse(userIdFromToken, out int userId))
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in or your user ID is invalid."
                });
            }

            var thread = await _context.Threads
                .Include(t => t.ThreadTags)
                .Include(t => t.ThreadImages)
                .FirstOrDefaultAsync(t => t.Id == threadId);

            if (thread == null)
            {
                return new NotFoundObjectResult(new
                {
                    error = "ThreadNotFound",
                    message = "Thread not found."
                });
            }

            if (thread.AuthorId != userId)
            {
                return new ForbidResult();
            }

            if (thread.PrimeThreadId == null && thread.SupThreadId == null)
            {
                var threadsToDelete = await _context.Threads
                    .Where(t => t.PrimeThreadId == thread.Id || t.SupThreadId == thread.Id || t.Id == thread.Id)
                    .ToListAsync();

                foreach (var t in threadsToDelete)
                {
                    t.Deleted = true;
                }
            }
            else
            {
                thread.Deleted = true;
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Thread deleted successfully." });
        }

        public async Task<IActionResult> LikeOrDislikeThread(int threadId, int likeOrDislike)
        {
            var userIdFromToken = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken) || !int.TryParse(userIdFromToken, out int userId))
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in or your user ID is invalid."
                });
            }

            if (likeOrDislike != 1 && likeOrDislike != -1)
            {
                return new BadRequestObjectResult(new
                {
                    error = "InvalidLikeValue",
                    message = "The likeOrDislike value must be 1 (like) or -1 (dislike)."
                });
            }

            var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId && !t.Deleted);

            if (thread == null)
            {
                return new NotFoundObjectResult(new
                {
                    error = "ThreadNotFound",
                    message = "The thread does not exist or has been deleted."
                });
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ThreadId == threadId);

            if (existingLike != null)
            {
                if (existingLike.LikeOrDislike == likeOrDislike)
                {
                    _context.Likes.Remove(existingLike);
                    await _context.SaveChangesAsync();
                    return new OkObjectResult(new { message = "Your vote has been removed." });
                }

                existingLike.LikeOrDislike = likeOrDislike;
                _context.Likes.Update(existingLike);
            }
            else
            {
                var newLike = new Likes
                {
                    UserId = userId,
                    ThreadId = threadId,
                    LikeOrDislike = likeOrDislike
                };

                await _context.Likes.AddAsync(newLike);
            }

            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Your vote has been recorded." });
        }

        public async Task<IActionResult> SubscribeThread(int threadId)
        {
            var userIdFromToken = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken) || !int.TryParse(userIdFromToken, out int userId))
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in or your user ID is invalid."
                });
            }

            var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId && !t.Deleted);
            if (thread == null || thread.PrimeThreadId != null || thread.SupThreadId != null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "InvalidThread",
                    message = "The specified thread does not exist or is not a main thread."
                });
            }

            var subscription = await _context.Subscriptions.FirstOrDefaultAsync(s => s.UserId == userId && s.ThreadId == threadId);

            if (subscription != null)
            {
                subscription.Subscribe = !subscription.Subscribe;
                _context.Subscriptions.Update(subscription);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    message = subscription.Subscribe ? "Subscribed successfully." : "Unsubscribed successfully."
                });
            }

            var newSubscription = new Subscriptions
            {
                UserId = userId,
                ThreadId = threadId,
                Subscribe = true
            };

            _context.Subscriptions.Add(newSubscription);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { message = "Subscribed successfully." });
        }

        public async Task<IActionResult> GetThreads(int pageNumber, int pageSize)
        {
            var threadsQuery = _context.Threads
                .Include(t => t.Author)
                .Include(t => t.ThreadTags) // Usuń ToList() tutaj
                    .ThenInclude(tt => tt.Tag) // Ładuj zagnieżdżone dane
                .Include(t => t.ThreadImages)
                .Where(t => !t.Deleted && t.SupThreadId == null && t.PrimeThreadId == null)
                .OrderByDescending(t => t.CreationDate);

            return await GetPaginatedThreads(threadsQuery, pageNumber, pageSize);
        }

        public async Task<IActionResult> GetUserThreads(int userId, int pageNumber, int pageSize)
        {
            var threadsQuery = _context.Threads
                .Include(t => t.Author)
                .Include(t => t.ThreadTags) // Usuń ToList() tutaj
                    .ThenInclude(tt => tt.Tag) // Ładuj zagnieżdżone dane
                .Include(t => t.ThreadImages)
                .Where(t => !t.Deleted && t.AuthorId == userId && t.SupThreadId == null && t.PrimeThreadId == null)
                .OrderByDescending(t => t.CreationDate);

            return await GetPaginatedThreads(threadsQuery, pageNumber, pageSize);
        }

        public async Task<IActionResult> GetMostDislikedThreads(int pageNumber, int pageSize)
        {
            var threadsQuery = _context.Threads
                .Include(t => t.Author)
                .Include(t => t.ThreadTags)
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.ThreadImages)
                .Where(t => !t.Deleted && t.SupThreadId == null && t.PrimeThreadId == null)
                .GroupJoin(
                    _context.Likes,
                    thread => thread.Id,
                    like => like.ThreadId,
                    (thread, likes) => new
                    {
                        Thread = thread,
                        TotalLikes = likes.Sum(l => l.LikeOrDislike)
                    })
                .Where(t => t.TotalLikes < 0)
                .Select(t => t.Thread);

            return await GetPaginatedThreads(threadsQuery, pageNumber, pageSize);
        }

        public async Task<IActionResult> GetSubscribedThreads(int pageNumber, int pageSize)
        {
            var userIdFromToken = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;

            if (string.IsNullOrEmpty(userIdFromToken) || !int.TryParse(userIdFromToken, out int userId))
            {
                return new BadRequestObjectResult(new
                {
                    error = "UserNotLogged",
                    message = "You aren't logged in or your user ID is invalid."
                });
            }

            var subscribedThreadsQuery = _context.Subscriptions
                .Where(s => s.UserId == userId && s.Subscribe)
                .Include(s => s.Thread)
                    .ThenInclude(t => t.Author)
                .Include(s => s.Thread)
                    .ThenInclude(t => t.ThreadTags)
                        .ThenInclude(tt => tt.Tag)
                .Include(s => s.Thread)
                    .ThenInclude(t => t.ThreadImages)
                .Select(s => s.Thread)
                .Where(t => !t.Deleted)
                .OrderByDescending(t => t.CreationDate);


            return await GetPaginatedThreads(subscribedThreadsQuery, pageNumber, pageSize);
        }

        public async Task<IActionResult> GetThreadAndSubthreads(int id)
        {
            var mainThread = await _context.Threads
                .Include(t => t.Author)
                .Include(t => t.ThreadTags)
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.ThreadImages)
                .FirstOrDefaultAsync(t => t.Id == id && t.PrimeThreadId == null && t.SupThreadId == null);

            if (mainThread == null || mainThread.Deleted)
            {
                return new NotFoundObjectResult(new
                {
                    error = "ThreadNotFound",
                    message = "The requested thread does not exist or has been deleted."
                });
            }

            var allThreads = await _context.Threads
                .Include(st => st.Author)
                .Include(st => st.ThreadTags)
                    .ThenInclude(tt => tt.Tag)
                .Include(st => st.ThreadImages)
                .ToListAsync();

            var threadDTO = MapThreadToDTO(mainThread, allThreads);

            return new OkObjectResult(threadDTO);
        }

        public async Task<IActionResult> SearchThread(string keyWord, int pageNumber, int pageSize)
        {
            var threadsQuery = _context.Threads
                .Include(t => t.Author)
                .Include(t => t.ThreadTags!.ToList())
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.ThreadImages)
                .Where(t => !t.Deleted &&
                    (t.Title.Contains(keyWord) ||
                    t.ThreadTags!.Any(tt => tt.Tag.Tag.Contains(keyWord))))
                .OrderByDescending(t => t.CreationDate);

            return await GetPaginatedThreads(threadsQuery, pageNumber, pageSize);
        }

        public async Task<IActionResult> GetThreadImage(int threadId, string filePath)
        {
            var thread = await _context.Threads.FirstOrDefaultAsync(t => t.Id == threadId);

            if (thread == null || thread.Deleted)
            {
                return new NotFoundObjectResult(new
                {
                    error = "ThreadNotFound",
                    message = "Thread not found or deleted."
                });
            }

            var fileName = Path.Combine("Images", "Threads", threadId.ToString(), filePath);

            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return new OkObjectResult(new
                {
                    profilePicture = (string?)null
                });
            }

            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var fileExtension = Path.GetExtension(fileName).ToLower();
            var contentType = fileExtension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            return new FileStreamResult(fileStream, contentType);
        }

        private GetThreadAndSubthreadsDTO MapThreadToDTO(Threads thread, List<Threads> allThreads)
        {
            var likesCount = _context.Likes.Count(l => l.ThreadId == thread.Id && l.LikeOrDislike > 0);
            var dislikesCount = _context.Likes.Count(l => l.ThreadId == thread.Id && l.LikeOrDislike < 0);

            List<string> newImages = new List<string>();
            if (thread.ThreadImages.Count() > 0)
            {
                foreach (var threadImage in thread.ThreadImages)
                {
                    newImages.Add(BusinessHelper.GenUrlThread(threadImage.Image, thread.Id));
                }
            }

            return new GetThreadAndSubthreadsDTO
            {
                ThreadId = thread.Id,
                Title = thread.Title,
                Description = thread.Description,
                AuthorNickname = thread.Author.Nickname,
                AuthorProfilePicture = BusinessHelper.GenUrlUser(thread.Author.ProfilePicture, thread.Author.Id),
                CreationDate = thread.CreationDate,
                Deleted = thread.Deleted,
                Tags = thread.ThreadTags?.Select(tt => tt.Tag.Tag).ToList(),
                Images = newImages ?? null,
                Likes = likesCount - dislikesCount,
                Subthreads = allThreads
                    .Where(subthread => subthread.SupThreadId == thread.Id && !subthread.Deleted)
                    .Select(subthread => MapThreadToDTO(subthread, allThreads))
                    .OrderBy(subthread => subthread.Title)
                    .ToList()
            };
        }

        private async Task<IActionResult> GetPaginatedThreads(IQueryable<Threads> query, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return new BadRequestObjectResult("Page number and size must be greater than zero.");
            }

            var userIdFromToken = _httpContextAccessor.HttpContext?.User.FindFirst("UserID")?.Value;
            int? userId = null;

            if (!string.IsNullOrEmpty(userIdFromToken) && int.TryParse(userIdFromToken, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            var totalCount = await query.CountAsync();

            var threads = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Dictionary<int, bool> subscriptions = new Dictionary<int, bool>();
            if (userId.HasValue)
            {
                var threadIds = threads.Select(t => t.Id).ToList();
                subscriptions = await _context.Subscriptions
                    .Where(s => threadIds.Contains(s.ThreadId) && s.UserId == userId)
                    .ToDictionaryAsync(s => s.ThreadId, s => s.Subscribe);
            }

            var threadDTOs = threads.Select(t => new GetThreadsDTO
            {
                ThreadId = t.Id,
                Title = t.Title,
                AuthorId = t.AuthorId,
                Author = t.Author.Nickname,
                Description = t.Description,
                CreationDate = t.CreationDate,
                Tags = t.ThreadTags != null && t.ThreadTags.Any() ? t.ThreadTags.Select(tt => tt.Tag.Tag).ToList() : null,
                Image = t.ThreadImages != null && t.ThreadImages.Any() ? BusinessHelper.GenUrlThread(t.ThreadImages.FirstOrDefault().Image, t.Id) : null,
                Subscribe = subscriptions.ContainsKey(t.Id) && subscriptions[t.Id]
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