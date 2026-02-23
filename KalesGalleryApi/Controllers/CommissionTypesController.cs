using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KalesGalleryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommissionTypesController : ControllerBase
{
    private readonly ICommissionTypeService _commissionTypeService;

    public CommissionTypesController(ICommissionTypeService commissionTypeService)
    {
        _commissionTypeService = commissionTypeService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CommissionType>>> GetAll()
    {
        var commissionTypes = await _commissionTypeService.GetAllAsync();
        return Ok(commissionTypes);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<CommissionType>> GetById(int id)
    {
        var commissionType = await _commissionTypeService.GetByIdAsync(id);
        if (commissionType == null) return NotFound();
        return Ok(commissionType);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CommissionType>> Create(CommissionType commissionType)
    {
        var created = await _commissionTypeService.CreateAsync(commissionType);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CommissionType>> Update(int id, CommissionType commissionType)
    {
        var updated = await _commissionTypeService.UpdateAsync(id, commissionType);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _commissionTypeService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
