using System.Text.Json.Serialization;

namespace KalesGalleryApi.Models;

public class Commission
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public decimal Price { get; set; }
    public bool IsCompleted { get; set; }

    // Navigation properties
    public ApplicationUser? User { get; set; }
    public CommissionType? Type { get; set; }
    [JsonIgnore]
    public Invoice? Invoice { get; set; }
}
