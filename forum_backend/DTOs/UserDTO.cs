using System.ComponentModel.DataAnnotations;

namespace forum_backend.DTOs
{
    public class UserDTO
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;

        public string EMail { get; set; } = null!;
    }
}
