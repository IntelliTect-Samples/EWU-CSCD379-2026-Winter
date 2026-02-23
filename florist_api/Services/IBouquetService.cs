using florist_api.Models;
using florist_api.DTOs;

namespace florist_api.Services
{
    public interface IBouquetService
    {
        Task<IEnumerable<Bouquet>> GetAllBouquetsAsync();
        Task<Bouquet?> GetByIdAsync(int id);
        Task<Bouquet> CreateBouquetAsync(BouquetCreateRequest bouquet); 
        
        Task<bool> UpdatePriceAsync(int id, decimal newPrice);
        Task<bool> DeleteAsync(int id);
    }
}