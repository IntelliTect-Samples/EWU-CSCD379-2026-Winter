using Microsoft.EntityFrameworkCore;
using WorkflowLite.Api.Data;
using WorkflowLite.Api.Dtos;
using WorkflowLite.Api.Models;

namespace WorkflowLite.Api.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly AppDbContext _db;
    public WorkOrderService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<WorkOrderListItemDto>> GetPublicBoardAsync(int take = 25)
    {
        // Public: safe fields only
        return await _db.WorkOrders
            .OrderByDescending(w => w.CreatedAt)
            .Take(take)
            .Select(w => new WorkOrderListItemDto(
                w.Id, w.Title, w.Status.ToString(), w.Priority.ToString(), w.CreatedAt))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<WorkOrderListItemDto>> GetMineAsync(string userId)
    {
        return await _db.WorkOrders
            .Where(w => w.CreatedByUserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WorkOrderListItemDto(
                w.Id, w.Title, w.Status.ToString(), w.Priority.ToString(), w.CreatedAt))
            .ToListAsync();
    }

    public async Task<WorkOrderDetailDto?> GetByIdAsync(int id, string? userId, bool isAdmin)
    {
        var w = await _db.WorkOrders.Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (w == null) return null;

        // Owner OR Admin can view details
        if (!isAdmin && userId != w.CreatedByUserId) return null;

        return new WorkOrderDetailDto(
            w.Id, w.Title, w.Description, w.Status.ToString(), w.Priority.ToString(),
            w.CreatedAt, w.CreatedByUserId,
            w.Comments.OrderByDescending(c => c.CreatedAt)
                .Select(c => new WorkOrderCommentDto(c.Id, c.Body, c.CreatedAt, c.CreatedByUserId))
                .ToList()
        );
    }

    public async Task<int> CreateAsync(string userId, CreateWorkOrderDto dto)
    {
        var priority = Enum.TryParse<Priority>(dto.Priority, true, out var p) ? p : Priority.Medium;

        var w = new WorkOrder
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Priority = priority,
            CreatedByUserId = userId
        };

        _db.WorkOrders.Add(w);
        await _db.SaveChangesAsync();
        return w.Id;
    }

    public async Task AddCommentAsync(int id, string userId, AddCommentDto dto, bool isAdmin)
    {
        var w = await _db.WorkOrders.FirstOrDefaultAsync(x => x.Id == id);
        if (w == null) throw new InvalidOperationException("Work order not found.");

        if (!isAdmin && w.CreatedByUserId != userId)
            throw new UnauthorizedAccessException();

        _db.WorkOrderComments.Add(new WorkOrderComment
        {
            WorkOrderId = id,
            Body = dto.Body.Trim(),
            CreatedByUserId = userId
        });

        await _db.SaveChangesAsync();
    }
}
