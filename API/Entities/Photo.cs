using System.Text.Json.Serialization;

namespace API.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? PublicId { get; set; } // For cloud storage services like Cloudinary

    // Navigation property to the Member entity
    [JsonIgnore]
    public Member Member { get; set; } = null!;

    // Foreign key to the Member entity
    public string MemberId { get; set; } = null!;
}
