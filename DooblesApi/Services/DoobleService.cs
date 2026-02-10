using DooblesApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DooblesApi.Services;

public class DoobleService : IDoobleService
{
    private readonly DooblesDbContext _context;
    private readonly Random _random = new();

    public DoobleService(DooblesDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetRandomDoobleNameAsync()
    {
  var count = await _context.DoobledNames.CountAsync();
  if (count == 0)
  {
        return null;
        }

        var randomIndex = _random.Next(count);
        var name = await _context.DoobledNames
    .OrderBy(d => d.Id)
   .Skip(randomIndex)
   .FirstOrDefaultAsync();

        return name?.Name;
    }

    public async Task<List<string>> GetAllNamesAsync()
    {
        return await _context.DoobledNames
     .OrderBy(d => d.Id)
            .Select(d => d.Name)
       .ToListAsync();
    }
}
