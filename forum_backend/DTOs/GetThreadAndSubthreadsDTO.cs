namespace forum_backend.DTOs
{
    public class GetThreadAndSubthreadsDTO
    {
        public int ThreadId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string AuthorNickname { get; set; } = null!;
        public int AuthorId { get; set; }
        public string? AuthorProfilePicture { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public bool Deleted { get; set; }
        public List<string>? Tags { get; set; }
        public List<string>? Images { get; set; }
        public int Likes { get; set; }
        public List<GetThreadAndSubthreadsDTO>? Subthreads { get; set; }
    }
}
