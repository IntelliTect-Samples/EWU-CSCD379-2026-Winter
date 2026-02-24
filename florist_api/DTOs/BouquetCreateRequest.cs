namespace florist_api.DTOs
{
    public class BouquetCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Season { get; set; }
        public int InventoryCount { get; set; }
    }
}