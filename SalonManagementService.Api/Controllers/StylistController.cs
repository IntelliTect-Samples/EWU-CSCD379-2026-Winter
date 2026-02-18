using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalonManagementService.Api.Dtos;
using SalonManagementService.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace SalonManagementService.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StylistController(SalonDbContext dbContext) : ControllerBase
{
    [HttpGet("List")]
    public async Task<ActionResult<IEnumerable<StylistDto>>> List()
    {
        var stylists = await dbContext.Stylists
            .Where(s => s.IsActive)
            .Select(s => new StylistDto
            {
                StylistId = s.StylistId,
                Name = s.Name,
                PhoneNumber = s.PhoneNumber,
                ChairName = s.ChairName,
                WorkStartTime24H = s.WorkStartTime24H,
                WorkEndTime24H = s.WorkEndTime24H,
                ImageUrl = s.Image != null ? $"{Request.Scheme}://{Request.Host}/Stylist/Image/{s.StylistId}" : null,
                IsActive = s.IsActive
            })
            .ToListAsync();

        return Ok(stylists);
    }

    [HttpPost]
    public async Task<ActionResult<StylistDto>> UpsertStylist([FromBody] StylistDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Create new stylist
        if (!dto.StylistId.HasValue || dto.StylistId.Value == Guid.Empty)
        {
            var newStylist = new Stylist
            {
                StylistId = Guid.NewGuid(),
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber,
                ChairName = dto.ChairName,
                WorkStartTime24H = dto.WorkStartTime24H,
                WorkEndTime24H = dto.WorkEndTime24H,
                IsActive = true
            };

            dbContext.Stylists.Add(newStylist);
            await dbContext.SaveChangesAsync();

            return Ok(new StylistDto
            {
                StylistId = newStylist.StylistId,
                Name = newStylist.Name,
                PhoneNumber = newStylist.PhoneNumber,
                ChairName = newStylist.ChairName,
                WorkStartTime24H = newStylist.WorkStartTime24H,
                WorkEndTime24H = newStylist.WorkEndTime24H,
                ImageUrl = null,
                IsActive = newStylist.IsActive
            });
        }

        // Update existing stylist
        var existingStylist = await dbContext.Stylists
            .FirstOrDefaultAsync(s => s.StylistId == dto.StylistId.Value);

        if (existingStylist == null)
        {
            return NotFound();
        }

        existingStylist.Name = dto.Name;
        existingStylist.PhoneNumber = dto.PhoneNumber;
        existingStylist.ChairName = dto.ChairName;
        existingStylist.WorkStartTime24H = dto.WorkStartTime24H;
        existingStylist.WorkEndTime24H = dto.WorkEndTime24H;

        await dbContext.SaveChangesAsync();

        return Ok(new StylistDto
        {
            StylistId = existingStylist.StylistId,
            Name = existingStylist.Name,
            PhoneNumber = existingStylist.PhoneNumber,
            ChairName = existingStylist.ChairName,
            WorkStartTime24H = existingStylist.WorkStartTime24H,
            WorkEndTime24H = existingStylist.WorkEndTime24H,
            ImageUrl = existingStylist.Image != null ? $"{Request.Scheme}://{Request.Host}/Stylist/Image/{existingStylist.StylistId}" : null,
            IsActive = existingStylist.IsActive
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult> DeleteStylist(Guid id)
    {
        var stylist = await dbContext.Stylists
            .FirstOrDefaultAsync(s => s.StylistId == id);

        if (stylist == null)
        {
            return NotFound();
        }

        stylist.IsActive = false;
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("Image/{id}")]
    [Authorize]
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

    [HttpPut("Image/{id}")]
    [Authorize(Roles = Roles.Stylist)]
    public async Task<ActionResult> UpdateImage(Guid id, IFormFile image)
    {
        var stylist = await dbContext.Stylists
            .FirstOrDefaultAsync(s => s.StylistId == id);

        if (stylist == null)
        {
            return NotFound();
        }

        using var memoryStream = new MemoryStream();
        await image.CopyToAsync(memoryStream);
        stylist.Image = memoryStream.ToArray();

        await dbContext.SaveChangesAsync();

        return Ok();
    }
}
