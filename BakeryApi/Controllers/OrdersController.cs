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
    }
}