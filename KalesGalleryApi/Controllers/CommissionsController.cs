using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KalesGalleryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommissionsController : ControllerBase
{
    private readonly ICommissionService _commissionService;
    private readonly IInvoiceService _invoiceService;
    private readonly ICommissionTypeService _commissionTypeService;

    public CommissionsController(
        ICommissionService commissionService,
        IInvoiceService invoiceService,
        ICommissionTypeService commissionTypeService)
    {
        _commissionService = commissionService;
        _invoiceService = invoiceService;
        _commissionTypeService = commissionTypeService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Commission>>> GetAll()
    {
        var commissions = await _commissionService.GetAllAsync();
        return Ok(commissions);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<Commission>> GetById(int id)
    {
        var commission = await _commissionService.GetByIdAsync(id);
        if (commission == null) return NotFound();
        return Ok(commission);
    }

    /// <summary>
    /// Get all commissions for the currently authenticated user.
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Commission>>> GetMyCommissions()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var commissions = await _commissionService.GetByUserIdAsync(userId);
        return Ok(commissions);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Commission>> Create(Commission commission)
    {
        // Set UserId from authenticated user
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        commission.UserId = userId;

        // Look up the commission type to set the price from the medium
        var commissionType = await _commissionTypeService.GetByIdAsync(commission.TypeId);
        if (commissionType == null) return BadRequest("Invalid commission type.");
        commission.Price = commissionType.Price;

        var created = await _commissionService.CreateAsync(commission);

        // Auto-create an invoice for this commission
        var invoice = new Invoice
        {
            UserId = userId,
            CommissionId = created.Id,
            TotalPrice = commissionType.Price
        };
        await _invoiceService.CreateAsync(invoice);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Commission>> Update(int id, Commission commission)
    {
        var updated = await _commissionService.UpdateAsync(id, commission);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _commissionService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
