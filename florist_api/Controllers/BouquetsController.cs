using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using florist_api.Models;
using florist_api.Services;
using florist_api.DTOs;

[ApiController]
[Route("api/[controller]")]
public class BouquetsController : ControllerBase
{
    private readonly IBouquetService _service;

    public BouquetsController(IBouquetService service)
    {
        _service = service;
    }

    // PUBLIC: Anyone can see the catalog from the database
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Bouquet>>> GetAll()
    {
        return Ok(await _service.GetAllBouquetsAsync());
    }

    // AUTHORIZED: Customers, Employees, and Admins can view details
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Bouquet>> GetById(int id)
    {
        var bouquet = await _service.GetByIdAsync(id);
        if (bouquet == null) return NotFound();
        return Ok(bouquet);
    }

    // ADMIN ONLY: Only the admin can add new items
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Bouquet>> Create([FromForm] BouquetCreateRequest request)
    {
        var created = await _service.CreateBouquetAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ADMIN ONLY: Only the admin can alter prices
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/price")]
    public async Task<IActionResult> UpdatePrice(int id, [FromBody] decimal newPrice)
    {
        var success = await _service.UpdatePriceAsync(id, newPrice);
        if (!success) return NotFound();
        return NoContent();
    }

    // ADMIN & EMPLOYEE: Both users can adjust the inventory count
    [Authorize(Roles = "Admin,Employee")]
    [HttpPatch("{id}/inventory")]
    public async Task<IActionResult> UpdateInventory(int id, [FromBody] int count)
    {
        var success = await _service.UpdateInventoryAsync(id, count);
        if (!success) return NotFound();
        return NoContent();
    }

    // ADMIN ONLY: Only admin can delete items from inventory
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}