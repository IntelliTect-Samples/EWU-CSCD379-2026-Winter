using System.Text.Json.Serialization;

namespace KalesGalleryApi.Models;

public class Invoice
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int CommissionId { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation properties
    [JsonIgnore]
    public ApplicationUser? User { get; set; }
    public Commission? Commission { get; set; }
}
