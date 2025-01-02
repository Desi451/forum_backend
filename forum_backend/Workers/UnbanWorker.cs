using forum_backend.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace forum_backend.Workers
{
    public class UnbanWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UnbanWorker> _logger;
        public UnbanWorker(IServiceProvider serviceProvider, ILogger<UnbanWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(">UnbanWorker started!");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var usersToUnban = await context.Bans
                        .Include(b => b.BannedUser)
                        .Where(b => b.BanUntil <= DateTimeOffset.UtcNow && b.BannedUser.status == -1)
                        .ToListAsync(stoppingToken);

                    if (usersToUnban.Any())
                    {
                        foreach (var ban in usersToUnban)
                        {
                            ban.BannedUser.status = 0;

                            _logger.LogInformation($"User {ban.BannedUser.Nickname} has been unbanned.");
                        }

                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"An error occurred in UnbanWorker: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }

            _logger.LogInformation(">UnbanWorker stopped!");
        }
    }
}
