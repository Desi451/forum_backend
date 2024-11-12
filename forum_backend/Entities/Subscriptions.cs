using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace forum_backend.Entities
{
    public class Subscriptions
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
        public bool Subscribe { get; set; } = true;
    }
}