using KalesGalleryApi.Models;

namespace KalesGalleryApi.Services;

public interface IArtPieceService
{
    Task<IEnumerable<ArtPiece>> GetAllAsync();
    Task<ArtPiece?> GetByIdAsync(int id);
    Task<ArtPiece> CreateAsync(ArtPiece artPiece);
    Task<ArtPiece?> UpdateAsync(int id, ArtPiece artPiece);
    Task<bool> DeleteAsync(int id);
}
