using KalesGalleryApi.Models;
using KalesGalleryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KalesGalleryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return Ok(invoices);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Invoice>> GetById(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetByUserId(string userId)
    {
        var invoices = await _invoiceService.GetByUserIdAsync(userId);
        return Ok(invoices);
    }

    /// <summary>
    /// Get the current user's invoices.
    /// </summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetMyInvoices()
    {
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        var invoices = await _invoiceService.GetByUserIdAsync(userId);
        return Ok(invoices);
    }

    /// <summary>
    /// Get the invoice for a specific commission.
    /// </summary>
    [HttpGet("commission/{commissionId}")]
    [Authorize]
    public async Task<ActionResult<Invoice>> GetByCommissionId(int commissionId)
    {
        var invoice = await _invoiceService.GetByCommissionIdAsync(commissionId);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Invoice>> Create(Invoice invoice)
    {
        var created = await _invoiceService.CreateAsync(invoice);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Invoice>> Update(int id, Invoice invoice)
    {
        var updated = await _invoiceService.UpdateAsync(id, invoice);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _invoiceService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
