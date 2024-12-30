namespace forum_backend.DTOs
{
    public class GetReportedUserDTO
    {
        public int Id { get; set; }
        public string ReportedUserNickname { get; set; } = null!;
        public string ReportedUserLogin { get; set; } = null!;
        public string ReportedUserMail { get; set; } = null!;
        public string ReportingUserNickname { get; set; } = null!;
        public string ReportingUserLogin { get; set; } = null!;
        public string ReportingUserMail { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public DateTimeOffset ReportDate { get; set; }
    }
}
