using Microsoft.AspNetCore.Mvc;
using WorkflowLite.Api.Services;

namespace WorkflowLite.Api.Controllers;

[ApiController]
[Route("api/public")]
public class PublicBoardController : ControllerBase
{
    private readonly IWorkOrderService _service;
    public PublicBoardController(IWorkOrderService service) => _service = service;

    [HttpGet("board")]
    public async Task<IActionResult> Board() => Ok(await _service.GetPublicBoardAsync());
}
