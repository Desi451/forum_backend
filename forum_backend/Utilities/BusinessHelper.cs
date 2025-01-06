﻿namespace forum_backend.Utilities;

public static class BusinessHelper
{
    private static IConfiguration? _configuration;

    public static void Configure(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static string GenUrlThread(string path, int threadId)
    {
        if (_configuration == null)
        {
            throw new InvalidOperationException("BusinessHelper is not configured. Call Configure method first.");
        }

        var fileName = Path.GetFileName(path);
        var forumBackendHostAddress = _configuration["AppSettings:ForumBackendHostAddress"];
        return $"{forumBackendHostAddress}/images/threads/{threadId}/{fileName}";
    }
}