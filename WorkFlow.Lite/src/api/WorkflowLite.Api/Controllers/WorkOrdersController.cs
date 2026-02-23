using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WorkflowLite.Api.Dtos;
using WorkflowLite.Api.Hubs;
using WorkflowLite.Api.Services;

namespace WorkflowLite.Api.Controllers;

[ApiController]
[Route("api/workorders")]
[Authorize]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _service;
    private readonly IHubContext<WorkOrdersHub> _hub;

    public WorkOrdersController(IWorkOrderService service, IHubContext<WorkOrdersHub> hub)
    {
        _service = service;
        _hub = hub;
    }

    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _service.GetMineAsync(userId));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkOrderDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var id = await _service.CreateAsync(userId, dto);

        await _hub.Clients.All.SendAsync("WorkOrderCreated", new { id });
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");
        var result = await _service.GetByIdAsync(id, userId, isAdmin);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("{id:int}/comments")]
    public async Task<IActionResult> AddComment(int id, AddCommentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        await _service.AddCommentAsync(id, userId, dto, isAdmin);
        await _hub.Clients.All.SendAsync("WorkOrderUpdated", new { id });

        return NoContent();
    }
}
