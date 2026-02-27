using BakeryApi.Models;

namespace BakeryApi.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> UpdateStatusAsync(int id, string? status);
        Task<bool> DeleteOrderAsync(int id);
    }
}