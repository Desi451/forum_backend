namespace forum_backend.DTOs
{
    public class EditThreadDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
