using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Services;

public class ArtPieceService : IArtPieceService
{
    private readonly GalleryDbContext _context;

    public ArtPieceService(GalleryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ArtPiece>> GetAllAsync()
    {
        return await _context.ArtPieces.ToListAsync();
    }

    public async Task<ArtPiece?> GetByIdAsync(int id)
    {
        return await _context.ArtPieces.FindAsync(id);
    }

    public async Task<ArtPiece> CreateAsync(ArtPiece artPiece)
    {
        _context.ArtPieces.Add(artPiece);
        await _context.SaveChangesAsync();
        return artPiece;
    }

    public async Task<ArtPiece?> UpdateAsync(int id, ArtPiece artPiece)
    {
        var existing = await _context.ArtPieces.FindAsync(id);
        if (existing == null) return null;

        existing.Name = artPiece.Name;
        existing.Description = artPiece.Description;
        existing.Price = artPiece.Price;
        existing.IsAvailable = artPiece.IsAvailable;
        existing.ImageUrl = artPiece.ImageUrl;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var artPiece = await _context.ArtPieces.FindAsync(id);
        if (artPiece == null) return false;

        _context.ArtPieces.Remove(artPiece);
        await _context.SaveChangesAsync();
        return true;
    }
}
