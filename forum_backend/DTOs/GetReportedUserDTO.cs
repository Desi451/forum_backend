namespace forum_backend.DTOs
{
    public class GetReportedUserDTO
    {
        public int ReportId { get; set; }
        public int ReportedUserId { get; set; }
        public string ReportedUserNickname { get; set; } = null!;
        public string ReportedUserLogin { get; set; } = null!;
        public string ReportedUserMail { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public DateTimeOffset ReportDate { get; set; }
        public int ReportingUserId { get; set; }
        public string ReportingUserNickname { get; set; } = null!;
        public string ReportingUserLogin { get; set; } = null!;
        public string ReportingUserMail { get; set; } = null!;
    }
}
