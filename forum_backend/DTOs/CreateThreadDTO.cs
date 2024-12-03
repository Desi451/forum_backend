namespace forum_backend.DTOs
{
    public class CreateThreadDTO
    {
        public int userId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
        public List<string> Tags { get; set; } = new List<string>();
    }
}
