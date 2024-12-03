namespace forum_backend.DTOs
{
    public class UpdateNicknameDTO
    {
        public int Id { get; set; }
        public string Nickname { get; set; } = null!;
    }
}