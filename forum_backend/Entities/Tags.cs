using System.ComponentModel.DataAnnotations;

namespace forum_backend.Entities;

public class Tags
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Tag { get; set; } = null!;

    public List<ThreadTags> ThreadsTags { get; set; } = null!;
}