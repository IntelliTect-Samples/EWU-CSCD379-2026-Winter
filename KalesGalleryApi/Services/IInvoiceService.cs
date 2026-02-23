using KalesGalleryApi.Models;

namespace KalesGalleryApi.Services;

public interface IInvoiceService
{
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(int id);
    Task<IEnumerable<Invoice>> GetByUserIdAsync(string userId);
    Task<Invoice?> GetByCommissionIdAsync(int commissionId);
    Task<Invoice> CreateAsync(Invoice invoice);
    Task<Invoice?> UpdateAsync(int id, Invoice invoice);
    Task<bool> DeleteAsync(int id);
}
