namespace forum_backend.DTOs
{
    public class GetUserDTO
    {
        public string Nickname { get; set; } = null!;
        public string Login { get; set; } = null!;
        public DateTimeOffset CreationDate { get; set; }
        public string? ProfilePicture { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
        public int NoOfThreads { get; set; }
        public int Likes { get; set; }
    }
}
