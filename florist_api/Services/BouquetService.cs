using florist_api.Data;
using florist_api.Models;
using Microsoft.EntityFrameworkCore;

namespace florist_api.Services
{
    public class BouquetService : IBouquetService
    {
        private readonly AppDbContext _context;

        public BouquetService(AppDbContext context)
        {
            _context = context;
        }

        // Public & Staff: View all flowers
        public async Task<IEnumerable<Bouquet>> GetAllBouquetsAsync()
        {
            return await _context.Bouquets.ToListAsync();
        }

        // Customer & Staff: View specific details
        public async Task<Bouquet?> GetByIdAsync(int id)
        {
            return await _context.Bouquets.FindAsync(id);
        }

        // Admin: Add new bouquets
        public async Task<Bouquet> CreateBouquetAsync(Bouquet bouquet)
        {
            _context.Bouquets.Add(bouquet);
            await _context.SaveChangesAsync();
            return bouquet;
        }

        // Admin: Update prices specifically
        public async Task<bool> UpdatePriceAsync(int id, decimal newPrice)
        {
            var bouquet = await _context.Bouquets.FindAsync(id);
            if (bouquet == null) return false;

            bouquet.Price = newPrice;
            await _context.SaveChangesAsync();
            return true;
        }

        // Admin: Delete from inventory
        public async Task<bool> DeleteAsync(int id)
        {
            var bouquet = await _context.Bouquets.FindAsync(id);
            if (bouquet == null) return false;

            _context.Bouquets.Remove(bouquet);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}