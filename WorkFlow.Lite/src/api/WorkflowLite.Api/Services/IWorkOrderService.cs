using WorkflowLite.Api.Dtos;

namespace WorkflowLite.Api.Services;

public interface IWorkOrderService
{
    Task<IReadOnlyList<WorkOrderListItemDto>> GetPublicBoardAsync(int take = 25);
    Task<IReadOnlyList<WorkOrderListItemDto>> GetMineAsync(string userId);
    Task<WorkOrderDetailDto?> GetByIdAsync(int id, string? userId, bool isAdmin);

    Task<int> CreateAsync(string userId, CreateWorkOrderDto dto);
    Task AddCommentAsync(int id, string userId, AddCommentDto dto, bool isAdmin);
}
