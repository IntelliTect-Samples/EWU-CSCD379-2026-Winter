using DooblesApi.Data;
using Microsoft.EntityFrameworkCore;

namespace DooblesApi.Tests;

public abstract class TestBase : IDisposable
{
    protected readonly DooblesDbContext _context;

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<DooblesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DooblesDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}