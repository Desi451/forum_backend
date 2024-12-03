namespace forum_backend.DTOs
{
    public class UpdateEMailDTO
    {
        public int Id { get; set; }
        public string NewEMail { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
