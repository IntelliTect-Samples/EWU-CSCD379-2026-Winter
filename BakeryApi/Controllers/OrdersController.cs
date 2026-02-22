using Microsoft.AspNetCore.Mvc;
using BakeryApi.Data;
using BakeryApi.Models;
using Microsoft.EntityFrameworkCore; 

namespace BakeryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

public class StatusUpdateDto
{
    public string? Status { get; set; }
}

        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Orders.Add(order);
            _context.SaveChanges();
            return Ok(order);
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems) 
                .ToList();

            return Ok(orders);
        }

        [HttpPost("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            var order = _context.Orders.Find(id);

            if (order == null)
                return NotFound();

            order.Status = dto?.Status ?? order.Status;
            _context.SaveChanges();

            return Ok(order);
        }
    }
}