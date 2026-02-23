namespace KalesGalleryApi.Models;

public class CommissionType
{
    public int Id { get; set; }
    public string Medium { get; set; } = string.Empty;
    public decimal Price { get; set; }

    // Navigation property
    public ICollection<Commission> Commissions { get; set; } = new List<Commission>();
}
