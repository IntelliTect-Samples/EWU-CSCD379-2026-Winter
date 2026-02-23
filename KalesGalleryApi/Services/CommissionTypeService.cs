using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Services;

public class CommissionTypeService : ICommissionTypeService
{
    private readonly GalleryDbContext _context;

    public CommissionTypeService(GalleryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CommissionType>> GetAllAsync()
    {
        return await _context.CommissionTypes.ToListAsync();
    }

    public async Task<CommissionType?> GetByIdAsync(int id)
    {
        return await _context.CommissionTypes.FindAsync(id);
    }

    public async Task<CommissionType> CreateAsync(CommissionType commissionType)
    {
        _context.CommissionTypes.Add(commissionType);
        await _context.SaveChangesAsync();
        return commissionType;
    }

    public async Task<CommissionType?> UpdateAsync(int id, CommissionType commissionType)
    {
        var existing = await _context.CommissionTypes.FindAsync(id);
        if (existing == null) return null;

        existing.Medium = commissionType.Medium;
        existing.Price = commissionType.Price;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var commissionType = await _context.CommissionTypes.FindAsync(id);
        if (commissionType == null) return false;

        _context.CommissionTypes.Remove(commissionType);
        await _context.SaveChangesAsync();
        return true;
    }
}
