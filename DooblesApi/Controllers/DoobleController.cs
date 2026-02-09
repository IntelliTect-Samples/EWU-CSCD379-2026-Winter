using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DooblesApi.Data;

namespace DooblesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DoobleController : ControllerBase
{
    private readonly DooblesDbContext _context;
    private readonly Random _random = new();

  public DoobleController(DooblesDbContext context)
  {
    _context = context;
    }

    [HttpGet("dooblename")]
    public async Task<ActionResult<string>> GetDoobledName()
    {
        var count = await _context.DoobledNames.CountAsync();
        if (count == 0)
        {
      return NotFound("No names found in database");
     }

        var randomIndex = _random.Next(count);
        var name = await _context.DoobledNames
   .Skip(randomIndex)
         .FirstOrDefaultAsync();

        return Ok(name?.Name);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<string>>> GetAllNames()
    {
        var names = await _context.DoobledNames
            .Select(d => d.Name)
 .ToListAsync();
      
        return Ok(names);
    }
}
