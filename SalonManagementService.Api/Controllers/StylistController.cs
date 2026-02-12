using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonManagementService.Api.Dtos;
using SalonManagementService.Api.Models;

namespace SalonManagementService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StylistController(SalonDbContext dbContext) : ControllerBase
{
    [HttpGet("List")]
    public async Task<ActionResult<IEnumerable<StylistListDto>>> List()
    {
        var stylists = await dbContext.Stylists
            .Select(s => new StylistListDto
            {
                StylistId = s.StylistId,
                Name = s.Name,
                ImageUrl = s.Image != null ? $"/Stylist/Image/{s.StylistId}" : null
            })
            .ToListAsync();

        return Ok(stylists);
    }

    [HttpGet("Image/{id}")]
    public async Task<ActionResult> Image(Guid id)
    {
        byte[]? stylistImage = await dbContext.Stylists
            .Where(s => s.StylistId == id)
            .Select(s => s.Image)
            .FirstOrDefaultAsync();

        if (stylistImage == null)
        {
            return NotFound();
        }

        return File(stylistImage, "image/jpeg");
    }
}
