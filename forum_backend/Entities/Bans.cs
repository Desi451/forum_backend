using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace forum_backend.Entities
{
    public class Bans
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("BannedUser")]
        public int BannedUserId { get; set; }
        public Users BannedUser { get; set; } = null!;

        [ForeignKey("BanningModerator")]
        public int BanningModeratorId { get; set; }
        public Users BanningModerator { get; set; } = null!;

        [Required]
        public string Reason { get; set; } = null!;

        [Required]
        public DateTimeOffset BanDate { get; set; }

        [Required]
        public DateTimeOffset BanUntil { get; set; }
    }
}