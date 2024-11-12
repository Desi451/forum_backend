using System.ComponentModel.DataAnnotations;

namespace forum_backend.Entities
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nickname { get; set; } = null!;

        [Required]
        public string Login { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string EMail { get; set; } = null!;

        [Required]
        public DateTimeOffset CreationDate { get; set; }

        public string? ProfilePicture { get; set; }

        [Required]
        public int Role {  get; set; }

        [Required]
        public int status { get; set; }

        public List<Threads>? UserThreads { get; set; }
    }
}