namespace KalesGalleryApi.Models.Dto;

public class CreateArtPieceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public IFormFile? Image { get; set; }
}

public class UpdateArtPieceDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public IFormFile? Image { get; set; }
}
