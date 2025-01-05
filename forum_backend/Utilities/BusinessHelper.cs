namespace forum_backend.Utilities;

public static class BusinessHelper
{
    public static string GenUrlThread(string path, int threadId)
    {
        var fileName = Path.GetFileName(path);
        var forumBackendHostAddress = "http://localhost:5179";
        return $"{forumBackendHostAddress}/images/threads/{threadId}/{fileName}";
    }
}
