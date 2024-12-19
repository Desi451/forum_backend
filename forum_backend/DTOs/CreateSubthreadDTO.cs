namespace forum_backend.DTOs
{
    public class CreateSubthreadDTO
    {
        public string Description { get; set; } = string.Empty;
        public int ParentThreadId { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
