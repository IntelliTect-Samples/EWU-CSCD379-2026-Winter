using KalesGalleryApi.Models;

namespace KalesGalleryApi.Services;

public interface ICommissionTypeService
{
    Task<IEnumerable<CommissionType>> GetAllAsync();
    Task<CommissionType?> GetByIdAsync(int id);
    Task<CommissionType> CreateAsync(CommissionType commissionType);
    Task<CommissionType?> UpdateAsync(int id, CommissionType commissionType);
    Task<bool> DeleteAsync(int id);
}
