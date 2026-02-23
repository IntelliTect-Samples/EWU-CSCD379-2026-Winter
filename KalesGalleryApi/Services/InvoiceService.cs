using KalesGalleryApi.Data;
using KalesGalleryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace KalesGalleryApi.Services;

public class InvoiceService : IInvoiceService
{
    private readonly GalleryDbContext _context;

    public InvoiceService(GalleryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.User)
            .Include(i => i.Commission)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByIdAsync(int id)
    {
        return await _context.Invoices
            .Include(i => i.User)
            .Include(i => i.Commission)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetByUserIdAsync(string userId)
    {
        return await _context.Invoices
            .Include(i => i.Commission)
                .ThenInclude(c => c!.Type)
            .Where(i => i.UserId == userId)
            .ToListAsync();
    }

    public async Task<Invoice?> GetByCommissionIdAsync(int commissionId)
    {
        return await _context.Invoices
            .Include(i => i.Commission)
                .ThenInclude(c => c!.Type)
            .FirstOrDefaultAsync(i => i.CommissionId == commissionId);
    }

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<Invoice?> UpdateAsync(int id, Invoice invoice)
    {
        var existing = await _context.Invoices.FindAsync(id);
        if (existing == null) return null;

        existing.UserId = invoice.UserId;
        existing.CommissionId = invoice.CommissionId;
        existing.TotalPrice = invoice.TotalPrice;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }
}
