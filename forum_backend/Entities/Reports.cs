using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace forum_backend.Entities
{
    public class Reports
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ReportedUser")]
        public int ReportedUserId { get; set; }
        public Users ReportedUser { get; set; } = null!;

        [ForeignKey("ReportingUser")]
        public int ReportingUserId { get; set; }
        public Users ReportingUser { get; set; } = null!;

        [Required]
        public string Reason { get; set; } = null!;

        [Required]
        public DateTimeOffset ReportDate { get; set; }
    }
}