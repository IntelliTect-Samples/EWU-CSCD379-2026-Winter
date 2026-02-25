using Microsoft.EntityFrameworkCore;
using WorkflowLite.Api.Data;
using WorkflowLite.Api.Dtos;
using WorkflowLite.Api.Services;
using Xunit;

public class WorkOrderServiceTests
{
    private static AppDbContext MakeDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_CreatesWorkOrder()
    {
        using var db = MakeDb();
        var svc = new WorkOrderService(db);

        var id = await svc.CreateAsync("user-1", new CreateWorkOrderDto("Fix printer", "Paper jam", "High"));
        var board = await svc.GetPublicBoardAsync();

        Assert.Contains(board, x => x.Id == id && x.Title == "Fix printer");
    }
}