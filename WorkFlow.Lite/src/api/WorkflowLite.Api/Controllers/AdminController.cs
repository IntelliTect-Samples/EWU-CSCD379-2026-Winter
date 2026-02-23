using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkflowLite.Api.Data;

namespace WorkflowLite.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminController(AppDbContext db) => _db = db;

    [HttpGet("all")]
    public async Task<IActionResult> All() =>
        Ok(await _db.WorkOrders.OrderByDescending(w => w.CreatedAt).ToListAsync());
}
