using System.ComponentModel.DataAnnotations.Schema;

namespace forum_backend.Entities
{
    public class ThreadTags
    {
        [ForeignKey("Thread")]
        public int ThreadId {  get; set; }
        public Threads Thread { get; set; } = null!;

        [ForeignKey("Tag")]
        public int TagId { get; set; }
        public Tags Tag { get; set; } = null!;
    }
}