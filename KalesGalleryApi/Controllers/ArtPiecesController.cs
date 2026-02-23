using KalesGalleryApi.Models;
using KalesGalleryApi.Models.Dto;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KalesGalleryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtPiecesController : ControllerBase
{
    private readonly IArtPieceService _artPieceService;
    private readonly IBlobStorageService _blobStorageService;

    public ArtPiecesController(IArtPieceService artPieceService, IBlobStorageService blobStorageService)
    {
        _artPieceService = artPieceService;
        _blobStorageService = blobStorageService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ArtPiece>>> GetAll()
    {
        var artPieces = await _artPieceService.GetAllAsync();
        return Ok(artPieces);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ArtPiece>> GetById(int id)
    {
        var artPiece = await _artPieceService.GetByIdAsync(id);
        if (artPiece == null) return NotFound();
        return Ok(artPiece);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ArtPiece>> Create([FromForm] CreateArtPieceDto dto)
    {
        string imageUrl = string.Empty;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            using var stream = dto.Image.OpenReadStream();
            imageUrl = await _blobStorageService.UploadAsync(stream, fileName, dto.Image.ContentType);
        }

        var artPiece = new ArtPiece
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsAvailable = dto.IsAvailable,
            ImageUrl = imageUrl
        };

        var created = await _artPieceService.CreateAsync(artPiece);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ArtPiece>> Update(int id, [FromForm] UpdateArtPieceDto dto)
    {
        var existing = await _artPieceService.GetByIdAsync(id);
        if (existing == null) return NotFound();

        string imageUrl = existing.ImageUrl;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            // Delete old image if exists
            if (!string.IsNullOrEmpty(existing.ImageUrl))
            {
                var oldBlobName = Path.GetFileName(new Uri(existing.ImageUrl).LocalPath);
                await _blobStorageService.DeleteAsync(oldBlobName);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            using var stream = dto.Image.OpenReadStream();
            imageUrl = await _blobStorageService.UploadAsync(stream, fileName, dto.Image.ContentType);
        }

        var artPiece = new ArtPiece
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsAvailable = dto.IsAvailable,
            ImageUrl = imageUrl
        };

        var updated = await _artPieceService.UpdateAsync(id, artPiece);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var artPiece = await _artPieceService.GetByIdAsync(id);
        if (artPiece == null) return NotFound();

        // Delete image from blob storage
        if (!string.IsNullOrEmpty(artPiece.ImageUrl))
        {
            var blobName = Path.GetFileName(new Uri(artPiece.ImageUrl).LocalPath);
            await _blobStorageService.DeleteAsync(blobName);
        }

        await _artPieceService.DeleteAsync(id);
        return NoContent();
    }
}
