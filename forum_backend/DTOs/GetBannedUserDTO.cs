namespace forum_backend.DTOs
{
    public class GetBannedUserDTO
    {
        public int Id { get; set; }
        public string UserNickname { get; set; } = null!;
        public string UserLogin { get; set; } = null!;
        public string UserMail { get; set; } = null!;
        public string AdminNickname { get; set; } = null!;
        public string AdminLogin { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public DateTimeOffset BanDate { get; set; }
        public DateTimeOffset? BanUntil { get; set; }
    }
}