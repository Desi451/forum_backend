namespace forum_backend.DTOs
{
    public class BanUserDTO
    {
        public string Reason { get; set; } = null!;
        public DateTimeOffset? BannedUntil { get; set; }
    }
}