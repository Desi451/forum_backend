namespace forum_backend.DTOs
{
    public class GetBannedUsersDTO
    {
        public int BannedUserId { get; set; }
        public string BannedUserNickname { get; set; } = null!;
        public string BannedUserLogin { get; set; } = null!;
        public string BannedUserEMail { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public DateTimeOffset DateOfBan { get; set; }
        public DateTimeOffset? BannedUntil { get; set; }
        public int AdminId { get; set; }
        public string AdminNickname { get; set; } = null!;
        public string AdminLogin { get; set; } = null!;
    }
}
