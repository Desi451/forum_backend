namespace forum_backend.DTOs
{
    public class UpdatePasswordDTO
    {
        public int UserId { get; set; }
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
