using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace forum_backend.Entities
{
    public class Images
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Thread")]
        public int ThreadId { get; set; }
        public Threads Thread { get; set; } = null!;

        [Required]
        public string Image { get; set; } = null!;
    }
}