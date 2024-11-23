namespace forum_backend.DTOs
{
    public class UpdateLoginDTO
    {
        public int UserId { get; set; }
        public string NewLogin { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
