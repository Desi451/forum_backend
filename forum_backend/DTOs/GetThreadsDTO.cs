namespace forum_backend.DTOs
{
    public class GetThreadsDTO
    {
        public int ThreadId { get; set; }
        public string Title { get; set; } = null!;
        public int AuthorId { get; set; }
        public string AuthorNickname { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTimeOffset CreationDate { get; set; }
        public List<string>? Tags { get; set; }
        public string? Image { get; set; }
        public bool Subscribe { get; set; }
    }
}
