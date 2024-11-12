using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace forum_backend.Entities
{
    public class Likes
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual Users User { get; set; } = null!;

        [ForeignKey("Thread")]
        public int ThreadId { get; set; }
        public virtual Threads Thread { get; set; } = null!;

        [Required]
        public int LikeOrDislike { get; set; }
    }
}