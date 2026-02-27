using BakeryApi.Data;
using BakeryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BakeryApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        public async Task<Order?> UpdateStatusAsync(int id, string? status)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
                return null;

            if (!string.IsNullOrEmpty(status))
                order.Status = status;

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}