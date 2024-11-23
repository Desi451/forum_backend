namespace forum_backend.DTOs
{
    public class UpdateNicknameDTO
    {
        public int UserId { get; set; }
        public string NewNickname { get; set; } = null!;
    }
}