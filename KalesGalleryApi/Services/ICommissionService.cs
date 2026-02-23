using KalesGalleryApi.Models;

namespace KalesGalleryApi.Services;

public interface ICommissionService
{
    Task<IEnumerable<Commission>> GetAllAsync();
    Task<Commission?> GetByIdAsync(int id);
    Task<IEnumerable<Commission>> GetByUserIdAsync(string userId);
    Task<Commission> CreateAsync(Commission commission);
    Task<Commission?> UpdateAsync(int id, Commission commission);
    Task<bool> DeleteAsync(int id);
}
