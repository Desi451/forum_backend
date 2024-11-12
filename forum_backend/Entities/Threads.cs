using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace forum_backend.Entities
{
    public class Threads
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public Users Author { get; set; } = null!;

        [ForeignKey("SupThread")]
        public int? SupThreadId { get; set; }
        public Threads? SupThread { get; set; }

        [ForeignKey("PrimeThread")]
        public int? PrimeThreadId { get; set; }
        public Threads? PrimeThread { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public DateTimeOffset CreationDate { get; set; }

        [Required]
        public bool Deleted { get; set; } = false;

        public List<Images>? ThreadImages { get; set; }

        public List<ThreadTags>? ThreadTags { get; set; }
    }
}