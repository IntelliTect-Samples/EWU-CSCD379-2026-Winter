using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BakeryApi.Models;
using BakeryApi.Services;

namespace BakeryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public class StatusUpdateDto
        {
            public string? Status { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _orderService.CreateOrderAsync(order);
            return Ok(created);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPost("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            var updated = await _orderService.UpdateStatusAsync(id, dto?.Status);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var success = await _orderService.DeleteOrderAsync(id);

            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}