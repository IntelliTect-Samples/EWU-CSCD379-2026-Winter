using florist_api.Data;
using florist_api.Models;
using florist_api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace florist_api.Services
{
    public class BouquetService : IBouquetService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BouquetService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<Bouquet>> GetAllBouquetsAsync()
        {
            return await _context.Bouquets.ToListAsync();
        }

        public async Task<Bouquet?> GetByIdAsync(int id)
        {
            return await _context.Bouquets.FindAsync(id);
        }

        public async Task<Bouquet> CreateBouquetAsync(BouquetCreateRequest dto)
        {
            string finalImageUrl = dto.ImageUrl ?? "";

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
               var rootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        
                var uploadsFolder = Path.Combine(rootPath, "uploads");

                if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                finalImageUrl = $"/uploads/{fileName}";
            }

            var bouquet = new Bouquet
            {
                Name = dto.Name!,
                Price = dto.Price,
                ImageUrl = finalImageUrl,
                Season = dto.Season!,
                IsAvailable = true,
                InventoryCount = dto.InventoryCount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bouquets.Add(bouquet);
            await _context.SaveChangesAsync();
            return bouquet;
        }

        // Admin: Allow editing of details
        public async Task<bool> UpdateBouquetAsync(int id, Bouquet updatedData)
        {
            var bouquet = await _context.Bouquets.FindAsync(id);
            if (bouquet == null) return false;

            bouquet.Name = updatedData.Name!;
            bouquet.Season = updatedData.Season!;
            bouquet.ImageUrl = updatedData.ImageUrl!;
            bouquet.IsAvailable = updatedData.IsAvailable;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePriceAsync(int id, decimal newPrice)
        {
            var bouquet = await _context.Bouquets.FindAsync(id);
            if (bouquet == null) return false;

            bouquet.Price = newPrice;
            await _context.SaveChangesAsync();
            return true;
        }

        // Admin & Employee: Shared editing of inventory count
        public async Task<bool> UpdateInventoryAsync(int id, int count)
        {
            var bouquet = await _context.Bouquets.FindAsync(id);
            if (bouquet == null) return false;

            bouquet.InventoryCount = count;
            await _context.SaveChangesAsync();
            return true;
        }

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