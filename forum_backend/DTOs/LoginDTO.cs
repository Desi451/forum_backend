namespace forum_backend.DTOs
{
    public class LoginDTO
    {
        public string LoginOrEMail { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}