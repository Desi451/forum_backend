namespace forum_backend.DTOs
{
    public class UpdatePFPDTO
    {
        public int UserId { get; set; }
        public string? NewProfilePicture { get; set; }
        public bool RemoveProfilePicture { get; set; }
    }
}
