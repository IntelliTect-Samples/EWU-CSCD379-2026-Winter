namespace WorkflowLite.Api.Dtos;

public record WorkOrderListItemDto(
    int Id, string Title, string Status, string Priority, DateTime CreatedAt);

public record WorkOrderCommentDto(
    int Id, string Body, DateTime CreatedAt, string CreatedByUserId);

public record WorkOrderDetailDto(
    int Id, string Title, string Description, string Status, string Priority,
    DateTime CreatedAt, string CreatedByUserId, List<WorkOrderCommentDto> Comments);

public record CreateWorkOrderDto(string Title, string Description, string Priority);
public record AddCommentDto(string Body);
public record UpdateStatusDto(string Status);
public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);
public record AuthResponseDto(string Token);
