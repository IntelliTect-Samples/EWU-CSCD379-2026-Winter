using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Services;

public class CommissionService : ICommissionService
{
    private readonly GalleryDbContext _context;

    public CommissionService(GalleryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Commission>> GetAllAsync()
    {
        return await _context.Commissions
            .Include(c => c.Type)
            .ToListAsync();
    }

    public async Task<Commission?> GetByIdAsync(int id)
    {
        return await _context.Commissions
            .Include(c => c.Type)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Commission>> GetByUserIdAsync(string userId)
    {
        return await _context.Commissions
            .Include(c => c.Type)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<Commission> CreateAsync(Commission commission)
    {
        _context.Commissions.Add(commission);
        await _context.SaveChangesAsync();
        return commission;
    }

    public async Task<Commission?> UpdateAsync(int id, Commission commission)
    {
        var existing = await _context.Commissions.FindAsync(id);
        if (existing == null) return null;

        existing.Name = commission.Name;
        existing.Description = commission.Description;
        existing.TypeId = commission.TypeId;
        existing.Price = commission.Price;
        existing.IsCompleted = commission.IsCompleted;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var commission = await _context.Commissions.FindAsync(id);
        if (commission == null) return false;

        _context.Commissions.Remove(commission);
        await _context.SaveChangesAsync();
        return true;
    }
}