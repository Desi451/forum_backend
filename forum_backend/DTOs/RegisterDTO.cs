namespace forum_backend.DTOs
{
    public class RegisterDTO
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string EMail { get; set; } = null!;
    }
}