using Microsoft.AspNetCore.Mvc;
using DooblesApi.Services;

namespace DooblesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DoobleController : ControllerBase
{
    private readonly IDoobleService _doobleService;

    public DoobleController(IDoobleService doobleService)
    {
        _doobleService = doobleService;
    }

  [HttpGet("dooblename")]
  public async Task<ActionResult<string>> GetDoobledName()
    {
  var name = await _doobleService.GetRandomDoobleNameAsync();
     
     if (name == null)
{
     return NotFound("No names found in database");
 }

        return Ok(name);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<string>>> GetAllNames()
    {
        var names = await _doobleService.GetAllNamesAsync();
     return Ok(names);
    }
}
